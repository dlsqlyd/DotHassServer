using DotHass.Server.Abstractions;
using DotNetty.Codecs.Http;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
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
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DotHass.Server.Websocket
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class WebsocketServerService : IServerService
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
        private readonly ILogger<WebsocketServerService> _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventLoop"></param>
        /// <param name="provider"></param>
        /// <param name="logger"></param>
        public WebsocketServerService(IServiceProvider provider, IHostingEnvironment environment, ILogger<WebsocketServerService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //创建作用域,区分不同hosted之间的service
            this._serviceProvider = provider?.CreateScope().ServiceProvider ?? throw new ArgumentNullException(nameof(provider));

            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            }
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
                    .Option(ChannelOption.SoBacklog, 8192)//设置channelconfig
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler($"{Options.Name}-CONN"));
                        this.Handler(channel, this._serviceProvider, Options);
                    }))
                    .ChildAttribute<AppFunc>(appKey, app)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        if (tlsCertificate != null)
                        {
                            pipeline.AddLast(TlsHandler.Server(tlsCertificate));
                        }
                        pipeline.AddLast(new HttpServerCodec());
                        pipeline.AddLast(new HttpObjectAggregator(65536));

                        pipeline.AddLast(ActivatorUtilities.CreateInstance<WebSocketClientHandler>(this._serviceProvider, Options));

                        pipeline.AddLast(ActivatorUtilities.CreateInstance<WebSocketFrameHandler>(this._serviceProvider));

                        this.ChildHandler(channel, this._serviceProvider, Options);
                    }));

            BoundChannel = await bootstrap.BindAsync(IPAddress.Loopback, Options.Port);
            _logger.LogInformation($"{Options.Name}-启动完成");
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