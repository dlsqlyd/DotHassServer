using System.Net;

namespace DotHass.Middleware.Mvc.Action.Result
{
    public class OkResult : StatusCodeResult
    {
        private const int DefaultStatusCode = (int)HttpStatusCode.OK;

        /// <summary>
        /// Initializes a new instance of the <see cref="OkResult"/> class.
        /// </summary>
        public OkResult()
            : base(DefaultStatusCode)
        {
        }
    }
}