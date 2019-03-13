using DotHass.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
namespace DotHass.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = null;
            try
            {
                host = new HostBuilder().Start<StartUp>(args).Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Host build error:" + ex);
                Console.ReadKey();
            }

            try
            {
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("host run error:" + ex);
                Console.ReadKey();
            }
        }
    }
}
