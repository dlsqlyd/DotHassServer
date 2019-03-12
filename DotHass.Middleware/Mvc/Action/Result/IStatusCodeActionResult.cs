namespace DotHass.Middleware.Mvc.Action.Result
{
    /// <summary>
    /// Represents an <see cref="IActionResult"/> that when executed will
    /// produce an HTTP response with the specified <see cref="StatusCode"/>.
    /// </summary>
    public interface IStatusCodeActionResult : IActionResult
    {
        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        int? StatusCode { get; }
    }
}