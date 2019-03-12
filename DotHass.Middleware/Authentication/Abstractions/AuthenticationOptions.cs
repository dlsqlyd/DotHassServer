using System;

namespace DotHass.Middleware.Authentication.Abstractions
{
    public class AuthenticationOptions
    {
        /// <summary>
        /// The SlidingExpiration is set to true to instruct the handler to re-issue a new cookie with a new
        /// expiration time any time it processes a request which is more than halfway through the expiration window.
        /// </summary>
        public bool SlidingExpiration { get; set; }

        /// <summary>
        /// <para>
        /// Controls how much time the authentication ticket stored in the cookie will remain valid from the point it is created
        /// The expiration information is stored in the protected cookie ticket. Because of that an expired cookie will be ignored
        /// even if it is passed to the server after the browser should have purged it.
        /// </para>
        /// <para>
        /// This is separate from the value of <seealso cref="CookieOptions.Expires"/>, which specifies
        /// how long the browser will keep the cookie.
        /// </para>
        /// </summary>
        public TimeSpan ExpireTimeSpan { get; set; }
    }
}