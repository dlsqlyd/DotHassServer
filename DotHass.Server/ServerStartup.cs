using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHass.Server.Abstractions
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class ServerStartup : IDisposable
    {
        public IEventLoopGroup BossGroup;
        public IEventLoopGroup WorkerGroup;
        private readonly ILogger<ServerStartup> _logger;
        protected IServiceProvider Services;
        private IEnumerable<IServerService> _hostedServices;
        private NetOptions options;

        public ServerStartup(IServiceProvider provider, IOptions<NetOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;

            var bossLoopCount = this.options.BossLoopCount == 0 ? this.options.Servers.Count : this.options.BossLoopCount;
            int workLoopCount = this.options.WorkLoopCount == 0 ? Environment.ProcessorCount * 2 : this.options.WorkLoopCount;
            BossGroup = new MultithreadEventLoopGroup(bossLoopCount);
            WorkerGroup = new MultithreadEventLoopGroup(workLoopCount);

            InternalLoggerFactory.DefaultFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServerStartup>();
            Services = provider;
        }

        public async Task StartAsync(AppFunc app)
        {
            _hostedServices = Services.GetService<IEnumerable<IServerService>>();

            foreach (var hostedService in _hostedServices)
            {
                var serverOptions = options.Servers.Find(o => o.Name == hostedService.Name);

                // Fire IHostedService.Start
                await hostedService.StartAsync(BossGroup, WorkerGroup, serverOptions, app).ConfigureAwait(false);
            }
        }

        public async Task StopAsync()
        {
            try
            {
                IList<Exception> exceptions = new List<Exception>();
                if (_hostedServices != null) // Started?
                {
                    foreach (var hostedService in _hostedServices.Reverse())
                    {
                        try
                        {
                            await hostedService.StopAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }

                if (exceptions.Count > 0)
                {
                    var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
                    _logger.LogError(ex, "Servers shutdown exception");
                    throw ex;
                }

                await Task.WhenAll(
                                   BossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                                   WorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Servers shutdown exception");
            }
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                StopAsync().Wait();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~ServerStartup()
        {
            Dispose(false);
        }
    }
}