using Owin;
using System;

namespace DotHass.Hosting
{
    public static class AppBuilderServicesExtensions
    {
        public static void SetServiceProvider(this IAppBuilder app, IServiceProvider serviceProvider)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.Properties[HostingConstants.HostServiceKey] = serviceProvider;
        }
    }
}