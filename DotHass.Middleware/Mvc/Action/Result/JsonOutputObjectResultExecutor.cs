using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DotHass.Middleware.Mvc.Action.Result
{
    /// <summary>
    /// Executes an <see cref="ObjectResult"/> to write to the response.
    /// </summary>
    public class JsonOutputObjectResultExecutor : IActionResultExecutor<ObjectResult>
    {
        /// <summary>
        /// Creates a new <see cref="ObjectResultExecutor"/>.
        /// </summary>
        /// <param name="formatterSelector">The <see cref="OutputFormatterSelector"/>.</param>
        /// <param name="writerFactory">The <see cref="IHttpResponseStreamWriterFactory"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public JsonOutputObjectResultExecutor(
            ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            Logger = loggerFactory.CreateLogger<JsonOutputObjectResultExecutor>();
        }

        /// <summary>
        /// Gets the <see cref="ILogger"/>.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Executes the <see cref="ObjectResult"/>.
        /// </summary>
        /// <param name="context">The <see cref="ActionContext"/> for the current request.</param>
        /// <param name="result">The <see cref="ObjectResult"/>.</param>
        /// <returns>
        /// A <see cref="Task"/> which will complete once the <see cref="ObjectResult"/> is written to the response.
        /// </returns>
        public virtual Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.StatusCode.HasValue)
            {
                context.MsgContext.Response.StatusCode = result.StatusCode.Value;
            }

            var str = JsonConvert.SerializeObject(result.Value);
            var data = Encoding.UTF8.GetBytes(str);

            context.MsgContext.Response.ContentType = "application/json";
            return context.MsgContext.Response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}