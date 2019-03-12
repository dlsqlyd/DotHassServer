using DotHass.Hosting.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotHass.Hosting
{
    public static class HostExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder Start<TStartup>(this IHostBuilder hostBuilder, string[] args) where TStartup : IStartup, new()
        {
            var startup = new TStartup().Initializtion(args);
            hostBuilder.ConfigureHostConfiguration(startup.ConfigureHostConfiguration);
            hostBuilder.ConfigureAppConfiguration(startup.ConfigureAppConfiguration);
            hostBuilder.ConfigureServices(startup.ConfigureServices);
            hostBuilder.ConfigureLogging(startup.ConfigureLogging);
            startup.Configure(hostBuilder);

            hostBuilder.ConfigureServices((services) =>
            {
                services.AddSingleton<IStartup>(startup);
            });
            return hostBuilder;
        }
    }
}