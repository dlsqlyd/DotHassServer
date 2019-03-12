namespace DotHass.Middleware.Mvc.Action.Result
{
    /// <summary>
    /// An <see cref="IStatusCodeActionResult"/> that can be transformed to a more descriptive client error.
    /// </summary>
    public interface IClientErrorActionResult : IStatusCodeActionResult
    {
    }
}