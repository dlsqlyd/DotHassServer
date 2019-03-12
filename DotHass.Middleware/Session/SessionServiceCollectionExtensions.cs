using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace DotHass.Middleware.Session
{
    /// <summary>
    /// Extension methods for adding session services to the DI container.
    /// </summary>
    public static class SessionServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for application session state.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddSession(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient<ISessionStore, DistributedSessionStore>();

            return services;
        }

        /// <summary>
        /// Adds services required for application session state.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configure">The session options to configure the middleware with.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddSession(this IServiceCollection services, Action<SessionOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);
            services.AddSession();

            return services;
        }
    }
}