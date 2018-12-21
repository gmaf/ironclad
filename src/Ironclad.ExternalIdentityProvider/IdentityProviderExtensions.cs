// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Extensions.DependencyInjection
{
    using AspNetCore.Authentication;
    using AspNetCore.Authentication.OpenIdConnect;
    using Ironclad.ExternalIdentityProvider;
    using Ironclad.ExternalIdentityProvider.Persistence;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class IdentityProviderExtensions
    {
        public static AuthenticationBuilder AddExternalIdentityProviders(this AuthenticationBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            builder.Services.AddTransient<IStore<IdentityProvider>, IdentityProviderStore>();
            builder.Services.AddTransient<IOpenIdConnectOptionsFactory, DefaultOpenIdConnectOptionsFactory>();
            builder.Services.AddTransientDecorator<IAuthenticationHandlerProvider, IdentityProviderAuthenticationHandlerProvider>();
            builder.Services.AddTransientDecorator<IAuthenticationSchemeProvider, IdentityProviderAuthenticationSchemeProvider>();
            return builder;
        }
    }
}
