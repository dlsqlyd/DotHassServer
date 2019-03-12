using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Action.Result
{
    public class ObjectResult : ActionResult, IStatusCodeActionResult
    {
        public ObjectResult(object value)
        {
            Value = value;
        }

        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var executor = context.MsgContext.Get<IServiceProvider>(Constants.RequestServiceKey).GetRequiredService<IActionResultExecutor<ObjectResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }
}