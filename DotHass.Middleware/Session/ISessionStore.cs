using DotHass.Middleware.Abstractions;
using System;

namespace DotHass.Middleware.Session
{
    public interface ISessionStore
    {
        ISession Create(string sessionKey, TimeSpan idleTimeout, TimeSpan ioTimeout, Func<bool> tryEstablishSession, bool isNewSessionKey);
    }
}