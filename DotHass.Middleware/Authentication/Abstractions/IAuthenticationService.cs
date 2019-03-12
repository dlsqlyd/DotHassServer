using System.Security.Claims;
using System.Threading.Tasks;

namespace DotHass.Middleware.Authentication.Abstractions
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// 认证中间件使用会刷新
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        Task<AuthenticateResult> AuthenticateAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Sign a principal in for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> to sign in.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        Task SignInAsync(ClaimsPrincipal principal, AuthenticationProperties properties);

        /// <summary>
        /// Sign out the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        Task SignOutAsync(string sessionKey);
    }
}