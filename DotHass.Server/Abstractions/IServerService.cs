using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotHass.Server.Abstractions
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public interface IServerService
    {
        string Name { get; set; }

        Action<IChannel, IServiceProvider, ServerOptions> Handler { get; set; }
        Action<IChannel, IServiceProvider, ServerOptions> ChildHandler { get; set; }

        //
        // 摘要:
        //     Triggered when the application host is ready to start the service.
        //
        // 参数:
        //   cancellationToken:
        //     Indicates that the start process has been aborted.
        Task StartAsync(IEventLoopGroup bossGroup, IEventLoopGroup workerGroup, ServerOptions options, AppFunc app);

        //
        // 摘要:
        //     Triggered when the application host is performing a graceful shutdown.
        //
        // 参数:
        //   cancellationToken:
        //     Indicates that the shutdown process should no longer be graceful.
        Task StopAsync();
    }
}