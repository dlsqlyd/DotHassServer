using DotHass.Middleware.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;

namespace DotHass.Middleware.Session
{
    public class DistributedSessionStore : ISessionStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILoggerFactory _loggerFactory;

        public DistributedSessionStore(IDistributedCache cache, ILoggerFactory loggerFactory)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache;
            _loggerFactory = loggerFactory;
        }

        public ISession Create(string sessionKey, TimeSpan idleTimeout, TimeSpan ioTimeout, Func<bool> tryEstablishSession, bool isNewSessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(sessionKey));
            }

            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            return new DistributedSession(_cache, sessionKey, idleTimeout, ioTimeout, tryEstablishSession, _loggerFactory, isNewSessionKey);
        }
    }
}