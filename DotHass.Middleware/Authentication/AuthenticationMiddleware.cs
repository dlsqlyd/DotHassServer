using DotHass.Middleware.Authentication.Abstractions;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotHass.Middleware.Authentication
{
    /// <summary>
    /// 认证中间件
    /// 设置上下文的user.供授权中间件使用
    /// 检查是否过期..未过期则续期
    /// 过期则删除。用户再发送请求。就不会有user。。授权就会拒绝
    /// </summary>
    public class AuthenticationMiddleware
    {
        public IAuthenticationService AuthService { get; set; }

        public AuthenticationMiddleware(IAuthenticationService authService)
        {
            this.AuthService = authService;
        }

        private Func<IDictionary<string, object>, Task> next;

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var user = environment[OwinConstants.Security.User] as ClaimsPrincipal;

            AuthenticateResult result = await AuthService.AuthenticateAsync(user);
            if (result?.Principal != null)
            {
                environment[OwinConstants.Security.User] = result.Principal;
            }
            await next(environment);
        }
    }
}