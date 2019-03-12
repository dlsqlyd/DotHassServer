using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotHass.Hosting.Startup
{
    public interface IStartup
    {
        IStartup Initializtion(string[] args);

        void ConfigureHostConfiguration(IConfigurationBuilder config);

        void ConfigureAppConfiguration(HostBuilderContext hostContext, IConfigurationBuilder config);

        void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services);

        void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder services);

        void Configure(IHostBuilder hostBuilder);
    }
}