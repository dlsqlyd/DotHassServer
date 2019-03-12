// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotHass.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DotHass.Server
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Implements the Katana setup pattern for the OwinHttpListener server.
    /// </summary>
    public static class OwinServerFactory
    {
        private static IServiceProvider serviceProvider;

        /// <summary>
        /// Advertise the capabilities of the server.
        /// </summary>
        /// <param name="properties"></param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by server later.")]
        public static void Initialize(IDictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            serviceProvider = properties.Get<IServiceProvider>(Constants.HostServiceKey);
        }

        /// <summary>
        /// Creates an OwinHttpListener and starts listening on the given URL.
        /// </summary>
        /// <param name="app">The application entry point.</param>
        /// <param name="properties">The addresses to listen on.</param>
        /// <returns>The OwinHttpListener.  Invoke Dispose to shut down.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller")]
        public static IDisposable Create(AppFunc app, IDictionary<string, object> properties)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            var startup = serviceProvider.GetService<ServerStartup>();
            startup.StartAsync(app).Wait();
            return startup;
        }
    }
}