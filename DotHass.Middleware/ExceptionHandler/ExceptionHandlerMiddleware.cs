using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DotHass.Middleware.ExceptionHandler
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
        }

        private Func<IDictionary<string, object>, Task> next;

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the logic of the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> that completes when the middleware has completed processing.</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            try
            {
                await next(environment);
            }
            catch (Exception ex)
            {
                _logger.UnhandledException(ex);
                environment[OwinConstants.ResponseStatusCode] = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}