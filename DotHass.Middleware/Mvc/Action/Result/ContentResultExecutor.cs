using System;
using System.Text;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Action.Result
{
    public class ContentResultExecutor : IActionResultExecutor<ContentResult>
    {
        public ContentResultExecutor()
        {
        }

        /// <inheritdoc />
        public virtual Task ExecuteAsync(ActionContext context, ContentResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.MsgContext.Response;

            if (result.StatusCode != null)
            {
                response.StatusCode = result.StatusCode.Value;
            }

            var data = Encoding.UTF8.GetBytes(result.Content);
            context.MsgContext.Response.ContentType = "text/plain";
            return response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}