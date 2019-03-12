// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Message;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading;

namespace DotHass.Server.RequestProcessing
{
    internal class OwinListenerContext : IDisposable
    {
        private readonly ConnectionInfo _listenerContext;

        private readonly CallEnvironment _environment;

        private CancellationTokenSource _cts;
        private CancellationTokenRegistration _disconnectRegistration;

        internal OwinListenerContext(ConnectionInfo listenerContext, IRequestMessage request)
        {
            _listenerContext = listenerContext;
            _environment = new CallEnvironment
            {
                { Constants.RequestScheme, "" },//Uri.UriSchemeHttp
                { Constants.RequestMethod, "" },
                { Constants.RequestPathBase, "" },
                { Constants.RequestPath, "" },
                { Constants.RequestQueryString, "" },
                { Constants.RequestProtocol, "" },
                { Constants.RequestHeaders, new RequestHeadersDictionary(request) },
                { Constants.RequestBody, new ListenerStreamWrapper(request.InputStream) },

                { Constants.ResponseStatusCode, (int)HttpStatusCode.OK },
                { Constants.ResponseBody, new ListenerStreamWrapper(new MemoryStream()) },
                { Constants.ResponseHeaders, new ResponseHeadersDictionary() },

                { Constants.OwinVersion, "" },
                { Constants.CallCancelled, GetCallCancelled() },

                { Constants.CommonKeys.RemoteIpAddress, "" },
                { Constants.CommonKeys.RemotePort, "" },
                { Constants.CommonKeys.LocalIpAddress, "" },
                { Constants.CommonKeys.LocalPort, "" },
                { Constants.CommonKeys.IsLocal, "" }
            };

            var user = new ClaimsPrincipal();
            var Identity = new ClaimsIdentity("session");
            Identity.AddClaim(new Claim(ClaimTypes.Sid, _listenerContext.SessionId));
            user.AddIdentity(Identity);
            _environment.Add(Constants.Security.User, user);
        }

        internal CallEnvironment Environment
        {
            get { return _environment; }
        }

        internal void End(Exception ex)
        {
            if (ex != null)
            {
                // TODO: LOG
                // Lazy initialized
                if (_cts != null)
                {
                    try
                    {
                        _cts.Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (AggregateException)
                    {
                        // TODO: LOG
                    }
                }
            }

            End();
        }

        internal void End()
        {
            try
            {
                _disconnectRegistration.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // CTR.Dispose() may throw an ODE on 4.0 if the CTS has previously been disposed.  Removed in 4.5.
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _disconnectRegistration.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // CTR.Dispose() may throw an ODE on 4.0 if the CTS has previously been disposed.  Removed in 4.5.
                }
                if (_cts != null)
                {
                    _cts.Dispose();
                }
            }
        }

        // Lazy environment initialization

        public CancellationToken GetCallCancelled()
        {
            _cts = new CancellationTokenSource();

            return _cts.Token;
        }
    }
}