using DotHass.Middleware.Mvc.Action;
using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Middleware.Mvc.MessageModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Controller
{
    public class ControllerMiddleware
    {
        private readonly ILogger<ControllerMiddleware> logger;
        private readonly IServiceProvider _serviceprovider;
        private IControllerProvider controllerProvider;

        public ControllerMiddleware(IServiceProvider provider, IControllerProvider controllerProvider, ILogger<ControllerMiddleware> logger)
        {
            this.logger = logger;
            this._serviceprovider = provider;
            this.controllerProvider = controllerProvider;
        }

        private Func<IDictionary<string, object>, Task> next;

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestHeaders = environment[OwinConstants.RequestHeaders] as IDictionary<string, string[]>;
            var contractid = Convert.ToInt32(requestHeaders[Constants.Headers.ContractId].First());

            var context = new OwinContext(environment);

            var controllerDesc = controllerProvider.GetControllerDescriptor(contractid);
            if (controllerDesc == null)
            {
                this.logger.LogError($"找不到指定的controller:{contractid}");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var controller = ActivatorUtilities.CreateInstance(this._serviceprovider, controllerDesc.ControllerType) as IMessageController;
            controller.Context = context;
            try
            {
                if (controllerDesc.HandlerBeforeMethodInfo != null && ExecuteBeforeHandlerMethod(controllerDesc.HandlerBeforeMethodInfo, controller, context) == false)
                {
                    return;
                }

                await ExecuteHandlerMethod(controllerDesc.HandlerMethodInfo, controller, context);

                if (controllerDesc.HandlerAfterMethodInfo != null)
                {
                    controllerDesc.HandlerAfterMethodInfo.Invoke(controller, new object[] { });
                }
            }
            finally
            {
                controller.Dispose();
            }
        }

        private async Task ExecuteHandlerMethod(MethodInfo handlerMethodInfo, IMessageController controller, OwinContext context)
        {
            var properties = PropertieBuilder.Build(handlerMethodInfo, context, out bool error);
            if (error == true)
            {
                return;
            }

            var obj = handlerMethodInfo.Invoke(controller, properties.Values.ToArray());
            IActionResult actionResult = null;

            if (obj is Task<IActionResult> actionTask)
            {
                actionResult = await actionTask;
            }
            else if (obj is IActionResult)
            {
                actionResult = obj as IActionResult;
            }

            var actionContext = new ActionContext(context, new ActionDescriptor() { }, properties, controller);
            await actionResult.ExecuteResultAsync(actionContext);
        }

        private bool ExecuteBeforeHandlerMethod(MethodInfo method, IMessageController controller, OwinContext context)
        {
            var beforeProperties = PropertieBuilder.Build(method, context, out bool berror);
            if (berror == true)
            {
                return false;
            }
            var beforeResult = (bool)method.Invoke(controller, beforeProperties.Values.ToArray());

            return beforeResult;
        }
    }
}