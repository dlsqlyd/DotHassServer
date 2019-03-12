using System;

namespace DotHass.Middleware.Session
{
    public class SessionOptions
    {
        /// <summary>
        /// The IdleTimeout indicates how long the session can be idle before its contents are abandoned. Each session access
        /// resets the timeout. Note this only applies to the content of the session, not the cookie.
        /// </summary>
        public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(20);

        /// <summary>
        /// The maximim amount of time allowed to load a session from the store or to commit it back to the store.
        /// Note this may only apply to asynchronous operations. This timeout can be disabled using <see cref="Timeout.InfiniteTimeSpan"/>.
        /// </summary>
        public TimeSpan IOTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}