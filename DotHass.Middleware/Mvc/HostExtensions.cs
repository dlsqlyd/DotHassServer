using DotHass.Middleware.Mvc.ApplicationParts;
using DotHass.Middleware.Mvc.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace DotHass.Middleware.Mvc
{
    public static class HostExtensions
    {
        public static IMvcCoreBuilder AddMvc(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var partManager = GetApplicationPartManager(services);
            services.TryAddSingleton(partManager);

            ConfigureDefaultFeatureProviders(partManager);
            AddMvcCoreServices(services);

            var builder = new MvcCoreBuilder(services, partManager);

            return builder;
        }

        private static void AddMvcCoreServices(IServiceCollection services)
        {
            services.AddScoped<IControllerProvider, ControllerProvider>();
        }

        private static void ConfigureDefaultFeatureProviders(ApplicationPartManager manager)
        {
            if (!manager.FeatureProviders.OfType<ControllerFeatureProvider>().Any())
            {
                manager.FeatureProviders.Add(new ControllerFeatureProvider());
            }
        }

        private static ApplicationPartManager GetApplicationPartManager(IServiceCollection services)
        {
            var manager = services.GetServiceFromCollection<ApplicationPartManager>();
            if (manager == null)
            {
                manager = new ApplicationPartManager();
                var environment = GetServiceFromCollection<IHostingEnvironment>(services);
                var entryAssemblyName = environment?.ApplicationName;
                if (string.IsNullOrEmpty(entryAssemblyName))
                {
                    return manager;
                }

                manager.PopulateDefaultParts(entryAssemblyName);
            }

            return manager;
        }

        /// <summary>
        /// Adds an <see cref="ApplicationPart"/> to the list of <see cref="ApplicationPartManager.ApplicationParts"/> on the
        /// <see cref="IMvcCoreBuilder.PartManager"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/> of the <see cref="ApplicationPart"/>.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder AddApplicationPart(this IMvcCoreBuilder builder, Assembly assembly)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            builder.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(new AssemblyPart(assembly)));

            return builder;
        }

        /// <summary>
        /// Configures the <see cref="ApplicationPartManager"/> of the <see cref="IMvcCoreBuilder.PartManager"/> using
        /// the given <see cref="Action{ApplicationPartManager}"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="setupAction">The <see cref="Action{ApplicationPartManager}"/></param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder ConfigureApplicationPartManager(
            this IMvcCoreBuilder builder,
            Action<ApplicationPartManager> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            setupAction(builder.PartManager);

            return builder;
        }

        public static T GetServiceFromCollection<T>(this IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}