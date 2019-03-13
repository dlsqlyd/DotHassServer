using DotHass.Hosting;
using DotHass.Hosting.Startup;
using DotHass.Identity;
using DotHass.Identity.Services;
using DotHass.Middleware.Authentication;
using DotHass.Middleware.Authentication.Abstractions;
using DotHass.Middleware.Authorization;
using DotHass.Middleware.ExceptionHandler;
using DotHass.Middleware.MiddlewareAnalysis;
using DotHass.Middleware.Mvc;
using DotHass.Middleware.Mvc.Action.Result;
using DotHass.Middleware.Mvc.Controller;
using DotHass.Middleware.Session;
using DotHass.Repository;
using DotHass.Sample.Data;
using DotHass.Sample.Model.Identity;
using DotHass.Sample.Service;
using DotHass.Server;
using DotHass.Server.Handler;
using DotHass.Server.Tcp;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using IdGen;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Owin;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

[assembly: OwinStartup("DotHass.Sample.StartUp", typeof(DotHass.Sample.StartUp), "Configuration")]
namespace DotHass.Sample
{
    public class StartUp : DefaultStartUp
    {
        public override void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            base.ConfigureServices(hostContext, services);

            services.AddSingleton<IHostedService, Application>();

            //其他常用的工具类
            //services.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();

            var Configuration = hostContext.Configuration;


            //idgen
            services.AddTransient<IdGenerator>((IServiceProvider provider) =>
            {
                var options = provider.GetService<IOptions<StartOptions>>().Value;
                return new IdGenerator(options.Id);
            });

            //cache
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Repository:redis:0").GetValue<string>("connectionString");
                options.InstanceName = "";
            });

            //identity
            RegisterIdentity(services, Configuration);

            // session
            services.AddSession((configure) =>
            {
                configure.IdleTimeout = TimeSpan.FromMinutes(20);
                configure.IOTimeout = TimeSpan.FromMinutes(1);
            });

            //mvc
            services.AddMvc();
            services.AddScoped<IActionResultExecutor<ObjectResult>, JsonOutputObjectResultExecutor>();
            services.AddScoped<IActionResultExecutor<ContentResult>, ContentResultExecutor>();

            this.RegisterGameServices(services, Configuration);
        }



        public void RegisterIdentity(IServiceCollection services, IConfiguration Configuration)
        {
            //auth
            services.Configure<AuthenticationOptions>((o) =>
            {
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITicketStore, DistributedCacheTicketStore>();
            services.AddScoped<ISystemClock, SystemClock>();
            services.AddScoped<AuthenticationMiddleware>();

            //Identity
            services.AddIdentityCore<GameUser>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<GameIdentityDbContext>()
                    .AddSignInManager()
                    .AddDefaultTokenProviders();
            services.AddScoped(typeof(IIdentityService<>), typeof(IdentityService<>));
        }

        public void RegisterGameServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<RoleService>();
            services.AddScoped<GangService>();

        }

        public override void Configure(IHostBuilder hostBuilder)
        {
            hostBuilder.AddDBContext<GameIdentityDbContext>(GameIdentityDbContext.ConnectionStringName)
                        .AddDBContext<GameDataDbContext>(GameDataDbContext.ConnectionStringName)
                        .AddDBContext<GameLogDbContext>(GameLogDbContext.ConnectionStringName);

            hostBuilder.AddRepository();

            hostBuilder.AddCacheManager();

            hostBuilder.AddServer()
                        .AddNettyServer<TcpServerService>("game", (channel, provider, options) =>
                        {
                            channel.Pipeline.AddLast(new LoggingHandler($"{options.Name}-CONN"));
                        }, (channel, provider, options) =>
                        {
                            channel.Pipeline.AddFirst(new AnalysisHandler());
                            //channel.Pipeline.AddLast(provider.GetService<LoggingHandler>());
                            channel.Pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2, true));
                            channel.Pipeline.AddLast("framing-enc", new LengthFieldPrepender(ByteOrder.LittleEndian, 2, 0, false));
                            channel.Pipeline.AddLast(provider.GetService<TcpClientHandler>());
                            channel.Pipeline.AddLast(provider.GetService<SecurityChannelHandler>());
                            channel.Pipeline.AddLast(provider.GetService<MessageChannelHandler>());
                        });
        }

        public void Configuration(IAppBuilder app)
        {
            var serviceProvider = app.Properties[HostingConstants.HostServiceKey] as IServiceProvider;
            app.Use(ActivatorUtilities.CreateInstance<ExceptionHandlerMiddleware>(serviceProvider));
            app.Use(ActivatorUtilities.CreateInstance<AnalysisMiddleware>(serviceProvider));
            app.Use(ActivatorUtilities.CreateInstance<AuthenticationMiddleware>(serviceProvider));
            app.Use(ActivatorUtilities.CreateInstance<AuthorizationMiddleware>(serviceProvider));
            app.Use(ActivatorUtilities.CreateInstance<SessionMiddleware>(serviceProvider));
            app.Use(ActivatorUtilities.CreateInstance<ControllerMiddleware>(serviceProvider));
        }
    }
}