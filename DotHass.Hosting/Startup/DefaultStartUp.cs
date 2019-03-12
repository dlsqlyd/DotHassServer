using DotHass.Hosting.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Owin.Hosting.Builder;
using Microsoft.Owin.Hosting.Loader;
using Microsoft.Owin.Hosting.ServerFactory;
using Microsoft.Owin.Hosting.Tracing;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DotHass.Hosting.Startup
{
    public class DefaultStartUp : IStartup
    {
        private string[] args;

        public IStartup Initializtion(string[] args)
        {
            this.args = args;

            return this;
        }

        public virtual void ConfigureHostConfiguration(IConfigurationBuilder config)
        {
            config.AddEnvironmentVariables();
            config.AddCommandLine(args);
        }

        public virtual void ConfigureAppConfiguration(HostBuilderContext hostContext, IConfigurationBuilder config)
        {
            var env = hostContext.HostingEnvironment;
            env.ApplicationName = env.ApplicationName ?? Assembly.GetEntryAssembly()?.GetName().Name;

            Console.Title = env.ApplicationName + "-" + env.ContentRootPath;

            config.SetBasePath(env.ContentRootPath);
            //。 optional：该json文件是否是可选的(和选项没关系),如果optional为true,當檔案不存在就會拋出FileNotFoundException。
            config.AddJsonFile("Resources/AppSettings/appsettings.json", optional: true, reloadOnChange: true);
            config.AddJsonFile($"Resources/AppSettings/appsettings.{env.EnvironmentName}.json", optional: true);
            //重新添加命令行参数命令行参数最优先
            config.AddCommandLine(args);
        }

        public virtual void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var Configuration = hostContext.Configuration;

            var listener = new DiagnosticListener("DotHass");
            services.AddSingleton<DiagnosticListener>(listener);
            services.AddSingleton<DiagnosticSource>(listener);

            //主机
            services.Configure<HostOptions>(Configuration.GetSection("Start"));
            services.Configure<ConsoleLifetimeOptions>(Configuration.GetSection("Start"));
            services.Configure<StartOptions>(Configuration.GetSection("Start"));

            services.AddSingleton<IAppBuilderFactory, AppBuilderFactory>();
            services.AddSingleton<IAppActivator, AppActivator>();

            services.AddSingleton<IAppLoader, AppLoader>();
            services.AddSingleton<IAppLoaderFactory, AppLoaderFactory>();

            services.AddSingleton<IServerFactoryActivator, ServerFactoryActivator>();
            services.AddSingleton<IServerFactoryAdapter, ServerFactoryAdapter>();
            services.AddSingleton<IServerFactoryLoader, ServerFactoryLoader>();

            services.AddSingleton<ITraceOutputFactory, TraceOutputFactory>();

            services.AddSingleton<IHostedService, HostingEngine>();

            //文件提供器
            var physicalProvider = hostContext.HostingEnvironment.ContentRootFileProvider;
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
            services.AddSingleton<IFileProvider>(compositeProvider);

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        }

        public virtual void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder loggerFactory)
        {
            var Configuration = hostContext.Configuration;
            loggerFactory.AddConfiguration(Configuration.GetSection("Logging"));
            loggerFactory.AddConsole();
            //使用 System.Diagnostics.Debug.WriteLine()写到vs上的输出窗口
            loggerFactory.AddDebug();
            var fileConfig = Configuration.GetSection("Logging:File");
            fileConfig["PathFormat"] = Path.Combine(hostContext.HostingEnvironment.ContentRootPath, fileConfig["PathFormat"]);
            loggerFactory.AddFile(fileConfig);
        }

        public virtual void Configure(IHostBuilder hostBuilder)
        {
        }
    }
}