using DotHass.Server.Abstractions;
using DotHass.Server.Handler;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Server.Tcp
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Defines the <see cref="TcpServerService" />
    /// </summary>
    public class TcpServerService : IServerService
    {
        public string Name { get; set; }

        protected IChannel BoundChannel { get; set; }
        public Action<IChannel, IServiceProvider, ServerOptions> Handler { get; set; }
        public Action<IChannel, IServiceProvider, ServerOptions> ChildHandler { get; set; }

        protected IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// Defines the _logger
        /// </summary>
        private readonly ILogger<TcpServerService> _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventLoop"></param>
        /// <param name="provider"></param>
        /// <param name="logger"></param>
        public TcpServerService(IServiceProvider provider, IHostingEnvironment environment, ILogger<TcpServerService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //创建作用域,区分不同hosted之间的service
            this._serviceProvider = provider?.CreateScope().ServiceProvider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(IEventLoopGroup BossGroup, IEventLoopGroup WorkerGroup, ServerOptions Options, AppFunc app)
        {
            X509Certificate2 tlsCertificate = null;
            if (Options.IsSsl)
            {
                tlsCertificate = new X509Certificate2(Path.Combine(_environment.ContentRootPath, Options.CertificatePath), Options.CertificateToken);
            }

            var bootstrap = new ServerBootstrap();
            var appKey = AttributeKey<AppFunc>.ValueOf(Constants.AppAttributeKey);
            bootstrap.Group(BossGroup, WorkerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)//设置channelconfig
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        this.Handler(channel, this._serviceProvider, Options);
                        channel.Pipeline.AddLast(ActivatorUtilities.CreateInstance<TcpChannelGroupHandler>(this._serviceProvider, Options));
                    }))
                    .ChildAttribute<AppFunc>(appKey, app)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        if (tlsCertificate != null)
                        {
                            channel.Pipeline.AddLast(TlsHandler.Server(tlsCertificate));
                        }
                        channel.Pipeline.AddLast(new IdleStateHandler(Options.Timeout, 0, 0));
                        channel.Pipeline.AddLast(new HeartBeatCheckHandler());

                        this.ChildHandler(channel, this._serviceProvider, Options);
                    }));

            BoundChannel = await bootstrap.BindAsync(Options.Port);
            _logger.LogInformation($"{Options.Name}-Server-启动完成.端口号:{Options.Port}");
        }

        /// <summary>
        /// The StopAsync
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task StopAsync()
        {
            // Stop called without start
            if (BoundChannel == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                await BoundChannel.CloseAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}