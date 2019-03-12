using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace DotHass.Tools.ExcelExport
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables();
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureHostConfiguration((configure) =>
                {
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppSettings>(hostContext.Configuration);
                    services.AddHostedService<ApplicationHostService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}