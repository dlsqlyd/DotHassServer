using DotHass.Middleware.Mvc.Controller;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace DotHass.Middleware.Authorization
{
    public static class AuthorizationStatusCodes
    {
        public const int Status1001AuthorizationFail = 1001;
    }

    /// <summary>
    /// 授权中间件
    /// 所有的控制器都需要基本的登录授权
    /// 有AuthorizeAttribute特性的进行额外的授权检查
    /// 有AllowAnonymous特性的不需要检查授权
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly ILogger<AuthorizationMiddleware> logger;

        private IControllerProvider controllerProvider;

        public AuthorizationMiddleware(IControllerProvider controllerProvider, ILogger<AuthorizationMiddleware> logger)
        {
            this.logger = logger;

            this.controllerProvider = controllerProvider;
        }

        private Func<IDictionary<string, object>, Task> next;

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestHeaders = environment[OwinConstants.RequestHeaders] as IDictionary<string, string[]>;

            var contractid = Convert.ToInt32(requestHeaders[Constants.Headers.ContractId].First());

            var controllerDesc = controllerProvider.GetControllerDescriptor(contractid);
            if (controllerDesc == null)
            {
                this.logger.LogError($"找不到指定的controller:{contractid}");
                environment[OwinConstants.ResponseStatusCode] = (int)HttpStatusCode.NotFound;
                return;
            }
            //检查是否定义了匿名特性
            environment.TryGetValue(OwinConstants.Security.User, out object user);
            if (controllerDesc.ControllerType.IsDefined(typeof(AllowAnonymousAttribute)) == false && user == null)
            {
                environment[OwinConstants.ResponseStatusCode] = AuthorizationStatusCodes.Status1001AuthorizationFail;
                return;
            }

            //特殊检查
            if (controllerDesc.ControllerType.IsDefined(typeof(AuthorizeAttribute)) == true)
            {
                //TODO:特殊检查
            }

            await next(environment);
        }
    }
}