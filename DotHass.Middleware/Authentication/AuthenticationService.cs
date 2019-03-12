using DotHass.Middleware.Authentication.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotHass.Middleware.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        protected AuthenticationOptions Options;

        protected ISystemClock Clock { get; }

        protected ITicketStore Store;

        public AuthenticationService(IOptions<AuthenticationOptions> options, ITicketStore store, ISystemClock clock)
        {
            Options = options.Value;
            Clock = clock;
            Store = store;
        }

        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        public virtual async Task<AuthenticateResult> AuthenticateAsync(ClaimsPrincipal principal)
        {
            var sid = principal.FindFirst(claim => claim.Type == ClaimTypes.Sid).Value;
            //根据sid恢复ticket
            var ticket = await Store.RetrieveAsync(sid);

            //如果没有，则失败
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Identity missing in session store");
            }

            var currentUtc = Clock.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                //过期。。认证失败
                await Store.RemoveAsync(sid);
                return AuthenticateResult.Fail("Ticket expired");
            }

            //刷新ticket
            await CheckForRefresh(sid, ticket);
            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        private async Task CheckForRefresh(string sid, AuthenticationTicket ticket)
        {
            var currentUtc = Clock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                if (timeRemaining < timeElapsed)
                {
                    await RequestRefresh(sid, ticket);
                }
            }
        }

        private async Task RequestRefresh(string sid, AuthenticationTicket ticket)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (issuedUtc != null && expiresUtc != null)
            {
                var properties = ticket.Properties;
                //设置发放时间
                var currentUtc = Clock.UtcNow;
                properties.IssuedUtc = currentUtc;
                //设置过期时间
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                properties.ExpiresUtc = currentUtc.Add(timeSpan);

                await Store.RenewAsync(sid, ticket);
            }
        }

        /// <summary>
        /// Sign a principal in for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> to sign in.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task SignInAsync(ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            properties = properties ?? new AuthenticationProperties();

            var sid = principal.FindFirst(claim => claim.Type == ClaimTypes.Sid).Value;

            DateTimeOffset issuedUtc;
            if (properties.IssuedUtc.HasValue)
            {
                issuedUtc = properties.IssuedUtc.Value;
            }
            else
            {
                issuedUtc = Clock.UtcNow;
                properties.IssuedUtc = issuedUtc;
            }

            if (!properties.ExpiresUtc.HasValue)
            {
                properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
            }

            //TODO:顶号,但不断线
            //根据令牌查找拥有同样令牌uid的sid.然后删除

            await Store.RemoveAsync(sid);

            var ticket = new AuthenticationTicket(principal, properties);

            await Store.StoreAsync(sid, ticket);
        }

        /// <summary>
        /// Sign out the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
        /// <returns>A task.</returns>
        public virtual async Task SignOutAsync(string sessionKey)
        {
            await Store.RemoveAsync(sessionKey);
        }
    }
}