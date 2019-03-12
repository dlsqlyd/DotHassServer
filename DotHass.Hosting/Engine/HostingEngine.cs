// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Builder;
using Microsoft.Owin.Hosting.Loader;
using Microsoft.Owin.Hosting.ServerFactory;
using Microsoft.Owin.Hosting.Tracing;
using Microsoft.Owin.Hosting.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Hosting.Engine
{
    /// <summary>
    /// Used to initialize and start a web application.
    /// </summary>
    public class HostingEngine : IHostedService
    {
        private readonly IAppBuilderFactory _appBuilderFactory;
        private readonly ITraceOutputFactory _traceOutputFactory;
        private readonly IAppLoader _appLoader;
        private readonly IServerFactoryLoader _serverFactoryLoader;
        private readonly IServiceProvider _serviceProvider;
        private readonly StartContext _context;
        private Disposable _disposable;

        /// <summary>
        ///
        /// </summary>
        /// <param name="appBuilderFactory"></param>
        /// <param name="traceOutputFactory"></param>
        /// <param name="appLoader"></param>
        /// <param name="serverFactoryLoader"></param>
        /// <param name="loggerFactory"></param>
        public HostingEngine(
            IServiceProvider serviceProvider,
            IAppBuilderFactory appBuilderFactory,
            ITraceOutputFactory traceOutputFactory,
            IAppLoader appLoader,
            IServerFactoryLoader serverFactoryLoader,
            IOptions<StartOptions> options)
        {
            _appBuilderFactory = appBuilderFactory ?? throw new ArgumentNullException("appBuilderFactory");
            _traceOutputFactory = traceOutputFactory ?? throw new ArgumentNullException("traceOutputFactory");
            _appLoader = appLoader ?? throw new ArgumentNullException("appLoader");
            _serverFactoryLoader = serverFactoryLoader;
            _serviceProvider = serviceProvider;
            this._context = new StartContext(options.Value);
        }

        private void ResolveOutput(StartContext context)
        {
            if (context.TraceOutput == null)
            {
                context.Options.Settings.TryGetValue("traceoutput", out string traceoutput);
                context.TraceOutput = _traceOutputFactory.Create(traceoutput);
            }

            context.EnvironmentData.Add(new KeyValuePair<string, object>(Constants.HostTraceOutput, context.TraceOutput));
        }

        private void InitializeBuilder(StartContext context)
        {
            if (context.Builder == null)
            {
                context.Builder = _appBuilderFactory.Create();
            }

            if (!string.IsNullOrWhiteSpace(context.Options.AppStartup))
            {
                context.Builder.Properties[Constants.HostAppName] = context.Options.AppStartup;
                context.EnvironmentData.Add(new KeyValuePair<string, object>(Constants.HostAppName, context.Options.AppStartup));
            }

            // This key lets us know the app was launched from Visual Studio.
            string vsVersion = Environment.GetEnvironmentVariable("VisualStudioVersion");
            if (!string.IsNullOrWhiteSpace(vsVersion))
            {
                context.Builder.Properties[Constants.HostAppMode] = Constants.AppModeDevelopment;
                context.EnvironmentData.Add(new KeyValuePair<string, object>(Constants.HostAppMode, Constants.AppModeDevelopment));
            }

            context.Builder.SetServiceProvider(_serviceProvider);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Partial trust not supported")]
        private void EnableTracing(StartContext context)
        {
            // string etwGuid = "CB50EAF9-025E-4CFB-A918-ED0F7C0CD0FA";
            // EventProviderTraceListener etwListener = new EventProviderTraceListener(etwGuid, "HostingEtwListener", "::");
            var textListener = new TextWriterTraceListener(context.TraceOutput, "HostingTraceListener");

            Trace.Listeners.Add(textListener);
            // Trace.Listeners.Add(etwListener);

            var source = new TraceSource("HostingTraceSource", SourceLevels.All);
            source.Listeners.Add(textListener);
            // source.Listeners.Add(etwListener);

            context.Builder.Properties[Constants.HostTraceOutput] = context.TraceOutput;
            context.Builder.Properties[Constants.HostTraceSource] = source;
        }

        private static IDisposable EnableDisposing(StartContext context)
        {
            var cts = new CancellationTokenSource();
            context.Builder.Properties[Constants.HostOnAppDisposing] = cts.Token;
            context.EnvironmentData.Add(new KeyValuePair<string, object>(Constants.HostOnAppDisposing, cts.Token));
            return new Disposable(() => cts.Cancel(false));
        }

        private void ResolveServerFactory(StartContext context)
        {
            if (context.ServerFactory != null)
            {
                return;
            }

            string serverName = DetermineOwinServer(context);
            context.ServerFactory = _serverFactoryLoader.Load(serverName);
        }

        private static string DetermineOwinServer(StartContext context)
        {
            StartOptions options = context.Options;
            IDictionary<string, string> settings = context.Options.Settings;

            string serverName = options.ServerFactory;
            if (!string.IsNullOrWhiteSpace(serverName))
            {
                return serverName;
            }

            if (settings != null &&
                settings.TryGetValue(Constants.SettingsOwinServer, out serverName) &&
                !string.IsNullOrWhiteSpace(serverName))
            {
                return serverName;
            }

            serverName = Environment.GetEnvironmentVariable(Constants.EnvOwnServer, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrWhiteSpace(serverName))
            {
                return serverName;
            }

            return Constants.DefaultServer;
        }

        private static string DetermineApplicationName(StartContext context)
        {
            StartOptions options = context.Options;
            IDictionary<string, string> settings = context.Options.Settings;

            if (options != null && !string.IsNullOrWhiteSpace(options.AppStartup))
            {
                return options.AppStartup;
            }

            if (settings.TryGetValue(Constants.SettingsOwinAppStartup, out string appName) &&
                !string.IsNullOrWhiteSpace(appName))
            {
                return appName;
            }

            return null;
        }

        private static void InitializeServerFactory(StartContext context)
        {
            context.ServerFactory.Initialize(context.Builder);
        }

        private void ResolveApp(StartContext context)
        {
            context.Builder.Use(typeof(Encapsulate), context.EnvironmentData);

            if (context.App == null)
            {
                IList<string> errors = new List<string>();
                if (context.Startup == null)
                {
                    string appName = DetermineApplicationName(context);
                    context.Startup = _appLoader.Load(appName, errors);
                }
                context.Startup(context.Builder);
            }
            else
            {
                context.Builder.Use(new Func<object, object>(_ => context.App));
            }
        }

        private static IDisposable StartServer(StartContext context)
        {
            return context.ServerFactory.Create(context.Builder);
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            ResolveOutput(_context);
            InitializeBuilder(_context);
            EnableTracing(_context);
            IDisposable disposablePipeline = EnableDisposing(_context);
            ResolveServerFactory(_context);
            InitializeServerFactory(_context);
            ResolveApp(_context);
            IDisposable disposableServer = StartServer(_context);

            this._disposable = new Disposable(
                () =>
                {
                    try
                    {
                        // first stop processing requests
                        disposableServer.Dispose();
                    }
                    finally
                    {
                        // then inform the pipeline of app shutdown
                        disposablePipeline.Dispose();
                        // Flush logs
                        _context.TraceOutput.Flush();
                    }
                });

            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            this._disposable.Dispose();
            return Task.CompletedTask;
        }
    }
}