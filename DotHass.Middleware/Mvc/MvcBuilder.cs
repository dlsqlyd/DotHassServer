using DotHass.Middleware.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotHass.Middleware.Mvc
{
    public class MvcCoreBuilder : IMvcCoreBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="MvcCoreBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="ApplicationPartManager"/> of the application.</param>
        public MvcCoreBuilder(IServiceCollection services, ApplicationPartManager manager)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            PartManager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public ApplicationPartManager PartManager { get; }
    }

    public interface IMvcCoreBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where MVC services are configured.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationPartManager"/> where <see cref="ApplicationPart"/>s
        /// are configured.
        /// </summary>
        ApplicationPartManager PartManager { get; }
    }
}