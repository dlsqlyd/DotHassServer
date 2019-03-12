using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Action.Result
{
    public class ContentResult : ActionResult, IStatusCodeActionResult
    {
        /// <summary>
        /// Gets or set the content representing the body of the response.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        public ContentResult(string content)
        {
            this.Content = content;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.MsgContext.Get<IServiceProvider>(Constants.RequestServiceKey).GetRequiredService<IActionResultExecutor<ContentResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }
}