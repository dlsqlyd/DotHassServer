using DotHass.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace DotHass.Identity
{
    public class IdentityService<TUser> : IIdentityService<TUser> where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IServiceProvider _provider;
        private IEmailSender _emailSender;
        private ISmsSender _smsSender;
        private ILogger<IdentityService<TUser>> _logger;

        public IdentityService(UserManager<TUser> userManager
            , SignInManager<TUser> signInManager
            , IServiceProvider provider
            , ILogger<IdentityService<TUser>> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._provider = provider;

            this._emailSender = this._provider.GetService<IEmailSender>();
            this._smsSender = this._provider.GetService<ISmsSender>();
            this._logger = logger;
        }

        public async Task<bool> Login(string name, string password, string sid)
        {
            var result = await _signInManager.PasswordSignInAsync(name, password, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(name);
                //登录成功后自动认证
                await SignInAsync(user, sid);
            }
            return result.Succeeded;
        }

        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task SignInAsync(TUser user, string sid)
        {
            await _signInManager.SignInAsync(user, sid);
        }

        /// <summary>
        /// 外部登陆
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public async Task<bool> ExternalLogin(string loginProvider, string providerKey, string sid)
        {
            //TODO：这里处理第三方登陆的一些消息。。比如请求获得用户数据等等。。。

            //然后更具获得的数据。。在获得用户数据

            var result = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
                await SignInAsync(user, sid);
            }
            return result.Succeeded;
        }

        #region 注册

        /// <summary>
        /// 普通注册
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Register(string name, string password)
        {
            TUser user = ActivatorUtilities.CreateInstance(_provider, typeof(TUser)) as TUser;
            user.UserName = name;
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        /// <summary>
        /// 两步注册
        /// 第一步，发送短信或者邮箱
        /// </summary>
        /// <param name="tokenProvider">TokenOptions.DefaultEmailProvider|| TokenOptions.DefaultPhoneProvider</param>
        /// <param name="tokenID">手机号码或者是邮箱</param>
        /// <returns></returns>
        public async Task<bool> TwoFactorSendCode(string tokenProvider, string tokenID)
        {
            var user = CreateUserByTokenProvider(tokenProvider, tokenID);
            //手机+密码+验证码   || 邮箱+密码+验证码
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var message = "Your security code is: " + code;
            if (tokenProvider == TokenOptions.DefaultEmailProvider)
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (tokenProvider == TokenOptions.DefaultPhoneProvider)
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return true;
        }

        public TUser CreateUserByTokenProvider(string tokenProvider, string tokenID)
        {
            TUser user = ActivatorUtilities.CreateInstance(_provider, typeof(TUser)) as TUser; ;

            if (tokenProvider == TokenOptions.DefaultEmailProvider)
            {
                user.Email = tokenID;
            }
            else if (tokenProvider == TokenOptions.DefaultPhoneProvider)
            {
                user.PhoneNumber = tokenID;
            }

            return user;
        }

        /// <summary>
        /// 第二步：验证第一步发送的tokenCode
        /// </summary>
        /// <param name="tokenProvider"></param>
        /// <param name="tokenID"></param>
        /// <param name="tokenCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> TwoFactorRegister(string tokenProvider, string tokenID, string tokenCode, string password)
        {
            var user = CreateUserByTokenProvider(tokenProvider, tokenID);

            //手机+密码+验证码   || 邮箱+密码+验证码
            if (await _userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, tokenCode) == false)
            {
                return false;
            }

            var result = await _userManager.CreateAsync(user, password);

            return result.Succeeded;
        }

        #endregion 注册

        public async Task<TUser> GetUser(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async void Update(TUser user)
        {
            await _userManager.UpdateAsync(user);
        }
    }
}