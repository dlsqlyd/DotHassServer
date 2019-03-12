using System.Net;

namespace DotHass.Middleware.Mvc.Action.Result
{
    public class OkObjectResult : ObjectResult
    {
        private const int DefaultStatusCode = (int)HttpStatusCode.OK;

        /// <summary>
        /// Initializes a new instance of the <see cref="OkObjectResult"/> class.
        /// </summary>
        /// <param name="value">The content to format into the entity body.</param>
        public OkObjectResult(object value)
            : base(value)
        {
            StatusCode = DefaultStatusCode;
        }
    }
}