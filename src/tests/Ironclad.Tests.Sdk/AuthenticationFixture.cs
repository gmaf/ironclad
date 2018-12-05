// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1600, CS1591 // not required for this class

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using IdentityModel.OidcClient;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using Xunit.Abstractions;

    public class AuthenticationFixture : IAsyncLifetime
    {
        private readonly IAsyncLifetime fixture;

        public AuthenticationFixture(IMessageSink messageSink = default)
        {
            // NOTE (Cameron):
            // The internally scoped Ironclad fixture manages the spinning up and tearing down of Ironclad and it's dependencies (postgres).
            // The publicly scoped authentication fixture is responsible for its lifetime which is why it is included here.
            this.fixture = new IroncladFixture(messageSink);

            var configuration = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.Authority = configuration.GetValue<string>("authority") ?? throw new ConfigurationErrorsException("Missing configuration value 'authority'");
        }

        public string Authority { get; }

        public HttpMessageHandler Handler { get; private set; }

        public async Task InitializeAsync()
        {
            await this.fixture.InitializeAsync().ConfigureAwait(false);

            // NOTE (Cameron): This automation is designed to use the default admin credentials (which need removing!) to log in to perform admin operations.
            var automation = new BrowserAutomation("admin", "password");
            var browser = new Browser(automation);
            var options = new OidcClientOptions
            {
                Authority = this.Authority,
                ClientId = "auth_console",
                RedirectUri = $"http://127.0.0.1:{browser.Port}",
                Scope = "openid profile auth_api",
                FilterClaims = false,
                Browser = browser,
                Policy = new Policy { Discovery = new DiscoveryPolicy { ValidateIssuerName = false } }
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);
            if (result.IsError)
            {
                throw new Exception(result.Error);
            }

            this.Handler = new TokenHandler(result.AccessToken);

            await this.OnInitializeAsync().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await this.OnDisposeAsync().ConfigureAwait(false);

            this.Handler?.Dispose();

            await this.fixture.DisposeAsync().ConfigureAwait(false);
        }

        protected virtual Task OnInitializeAsync() => Task.CompletedTask;

        protected virtual Task OnDisposeAsync() => Task.CompletedTask;

        private sealed class WaitUntilAvailableResult
        {
            public static readonly WaitUntilAvailableResult NotAvailable = new WaitUntilAvailableResult(null);

            private WaitUntilAvailableResult(string accessToken)
            {
                this.AccessToken = accessToken;
            }

            public string AccessToken { get; }

            public static WaitUntilAvailableResult Available(string accessToken) => new WaitUntilAvailableResult(accessToken);
        }

        private sealed class TokenHandler : DelegatingHandler
        {
            private readonly string accessToken;

            public TokenHandler(string accessToken)
                : base(new HttpClientHandler())
            {
                this.accessToken = accessToken;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken);
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}