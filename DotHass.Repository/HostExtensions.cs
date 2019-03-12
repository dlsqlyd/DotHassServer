using DotHass.Repository.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotHass.Repository
{
    public static class HostExtensions
    {
        public static IHostBuilder AddDBContext<TDBContext>(this IHostBuilder hostBuilder, string name, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TDBContext : DbContext
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var Configuration = hostContext.Configuration;

                var conntionstring = Configuration.GetConnectionString(name);

                services.AddDbContext<TDBContext>((options) =>
                {
                    options.UseMySql(conntionstring);
                }, contextLifetime, optionsLifetime);

                switch (name)
                {
                    case DataDbContext.ConnectionStringName:
                        services.AddSingleton(typeof(DataDbContext), typeof(TDBContext));
                        break;

                    case LogDbContext.ConnectionStringName:
                        services.AddSingleton(typeof(LogDbContext), typeof(TDBContext));
                        break;

                    default:
                        break;
                }
                services.AddSingleton<TDBContext, TDBContext>();
            });
        }

        public static IHostBuilder AddRepository(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var Configuration = hostContext.Configuration;
                //repos
                services.Configure<JsonConfigOptions>(Configuration.GetSection("Repository:JsonConfig"));

                services.AddScoped(typeof(DataDbRepository<>));

                services.AddScoped(typeof(LogDbRepository<>));

                services.AddScoped(typeof(JsonConfigRepository<>));

                services.AddScoped(typeof(CacheRepository<>));

                services.AddScoped(typeof(GroupCacheRepository<>));

                services.AddScoped(typeof(EfCoreRepository<,>));
            });
        }

        public static IHostBuilder AddCacheManager(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var Configuration = hostContext.Configuration;
                services.AddCacheManagerConfiguration(Configuration.GetSection("Repository"));
                services.AddCacheManager();
                services.AddScoped(typeof(DistributedCacheRepository<>));
            });
        }
    }
}