using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DotHass.Identity
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddDefaultTokenProviders(this IdentityBuilder builder)
        {
            var userType = builder.UserType;

            var phoneNumberProviderType = typeof(PhoneNumberTokenProvider<>).MakeGenericType(userType);
            var emailTokenProviderType = typeof(EmailTokenProvider<>).MakeGenericType(userType);
            var authenticatorProviderType = typeof(AuthenticatorTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType)
                .AddTokenProvider(TokenOptions.DefaultPhoneProvider, phoneNumberProviderType)
                .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, authenticatorProviderType);
        }

        public static IdentityBuilder AddSignInManager(this IdentityBuilder builder)
        {
            builder.AddSignInManagerDeps();
            var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(managerType);
            return builder;
        }

        private static void AddSignInManagerDeps(this IdentityBuilder builder)
        {
        }
    }
}