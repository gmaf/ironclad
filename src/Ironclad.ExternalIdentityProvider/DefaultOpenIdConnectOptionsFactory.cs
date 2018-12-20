// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.ExternalIdentityProvider
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Ironclad.ExternalIdentityProvider.Persistence;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public sealed class DefaultOpenIdConnectOptionsFactory : IOpenIdConnectOptionsFactory
    {
        private readonly IPostConfigureOptions<OpenIdConnectOptions> configureOptions;
        private readonly ILogger<DefaultOpenIdConnectOptionsFactory> logger;

        public DefaultOpenIdConnectOptionsFactory(IPostConfigureOptions<OpenIdConnectOptions> configureOptions, ILogger<DefaultOpenIdConnectOptionsFactory> logger)
        {
            this.configureOptions = configureOptions;
            this.logger = logger;
        }

        public OpenIdConnectOptions CreateOptions(IdentityProvider identityProvider)
        {
            this.logger.LogInformation($"Configuring {identityProvider.Name} identity provider");

            var options = new OpenIdConnectOptions
            {
                Authority = identityProvider.Authority,
                ClientId = identityProvider.ClientId,
                RequireHttpsMetadata = false, // NOTE (Cameron): We want to enable developers to use this for testing purposes.
            };

            options.CallbackPath = identityProvider.CallbackPath ?? options.CallbackPath;

            foreach (var scope in identityProvider.Scopes ?? Enumerable.Empty<string>())
            {
                if (!options.Scope.Contains(scope))
                {
                    options.Scope.Add(scope);
                }
            }

            if (identityProvider.AcrValues?.Count > 0)
            {
                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    context.ProtocolMessage.AcrValues = string.Join(" ", identityProvider.AcrValues);
                    return Task.CompletedTask;
                };
            }

            this.configureOptions.PostConfigure(identityProvider.Name, options);

            return options;
        }

        public IOptionsMonitor<OpenIdConnectOptions> CreateOptionsMonitor(IdentityProvider identityProvider) =>
            new StaticOptionsMonitor(this.CreateOptions(identityProvider));

        private class StaticOptionsMonitor : IOptionsMonitor<OpenIdConnectOptions>
        {
            private static readonly NullDisposable Disposable = new NullDisposable();

            public StaticOptionsMonitor(OpenIdConnectOptions options)
            {
                this.CurrentValue = options;
            }

            public OpenIdConnectOptions CurrentValue { get; }

            public OpenIdConnectOptions Get(string name) => this.CurrentValue;

            public IDisposable OnChange(Action<OpenIdConnectOptions, string> listener) => Disposable;

            private class NullDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}
