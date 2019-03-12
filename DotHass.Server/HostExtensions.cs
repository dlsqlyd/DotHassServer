// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Message;
using DotHass.Server.Handler;
using DotHass.Server.Http;
using DotHass.Server.Kcp;
using DotHass.Server.Message;
using DotHass.Server.Tcp;
using DotHass.Server.Udp;
using DotHass.Server.Websocket;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DotHass.Server
{
    public static class HostExtensions
    {
        public static IHostBuilder AddServer(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.Configure<NetOptions>(hostContext.Configuration.GetSection("Net"));
                services.Configure<SecurityOptions>(hostContext.Configuration.GetSection("Security"));
                services.AddSingleton<ServerStartup>();
                services.AddScoped<IConnectionChannelGroup, ConnectionChannelGroup>();

                services.AddTransient<UdpServerService>();
                services.AddTransient<TcpServerService>();
                services.AddTransient<HttpServerService>();
                services.AddTransient<WebsocketServerService>();

                //添加共享的ChannelHandler实例
                services.AddScoped<LoggingHandler>();
                services.AddScoped<TcpClientHandler>();
                services.AddScoped<UdpClientHandler>();
                services.AddScoped<KcpClientHandler>();
                services.AddScoped<MessageChannelHandler>();
                services.AddScoped<SecurityChannelHandler>();

                //消息处理
                services.AddScoped<IMessageHandler, DefaultMessageHandler>();
                services.AddScoped<IMessageFactory, MessageFactory<DefaultRequestMessagePacket, DefaultResponseMessagePacket>>();
            });
        }

        public static IHostBuilder AddNettyServer<T>(this IHostBuilder hostBuilder, string name, Action<IChannel, IServiceProvider, ServerOptions> handler, Action<IChannel, IServiceProvider, ServerOptions> childhandler) where T : IServerService
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IServerService>((provider) =>
                {
                    var host = provider.GetService<T>();
                    host.Name = name;
                    host.Handler = handler;
                    host.ChildHandler = childhandler;
                    return host;
                });
            });
        }
    }
}