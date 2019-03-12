using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DotHass.Middleware.Session
{
    /// <summary>
    /// Enables the session state for the application.
    /// </summary>
    public class SessionMiddleware
    {
        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        private static readonly Func<bool> ReturnTrue = () => true;
        private readonly SessionOptions _options;
        private readonly ILogger _logger;
        private readonly ISessionStore _sessionStore;

        public readonly string Region = "__Session";

        /// <summary>
        /// Creates a new <see cref="SessionMiddleware"/>.
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> representing the factory that used to create logger instances.</param>
        /// <param name="dataProtectionProvider">The <see cref="IDataProtectionProvider"/> used to protect and verify the cookie.</param>
        /// <param name="sessionStore">The <see cref="ISessionCache"/> representing the session store.</param>
        /// <param name="options">The session configuration options.</param>
        public SessionMiddleware(
            ILoggerFactory loggerFactory,
            ISessionStore sessionStore,
            IOptions<SessionOptions> options)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _logger = loggerFactory.CreateLogger<SessionMiddleware>();
            _options = options.Value;
            _sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));
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
            var isNewSessionKey = false;
            Func<bool> tryEstablishSession = ReturnTrue;

            var user = environment[OwinConstants.Security.User] as ClaimsPrincipal;

            var sessionKey = user.FindFirst(claim => claim.Type == ClaimTypes.Sid).Value;

            var session = _sessionStore.Create(Region + ":" + sessionKey, _options.IdleTimeout, _options.IOTimeout, tryEstablishSession, isNewSessionKey);

            environment["Middleware.Session"] = session;
            try
            {
                await next(environment);
            }
            finally
            {
                try
                {
                    //中间件结束后。。保存session
                    await session.CommitAsync();
                }
                catch (OperationCanceledException)
                {
                    _logger.SessionCommitCanceled();
                }
                catch (Exception ex)
                {
                    _logger.ErrorClosingTheSession(ex);
                }
            }
        }
    }
}