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

    public sealed class AuthenticationFixture : IAsyncLifetime
    {
        private readonly IAsyncLifetime fixture;

        private readonly string username;
        private readonly string password;
        private readonly string clientId;
        private readonly string scope;

        public AuthenticationFixture(IMessageSink messageSink)
        {
            // NOTE (Cameron):
            // The internally scoped Ironclad fixture manages the spinning up and tearing down of Ironclad and it's dependencies (postgres).
            // The publicly scoped authentication fixture is responsible for its lifetime which is why it is included here.
            this.fixture = new IroncladFixture(messageSink);

            var configuration = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.Authority = configuration.GetValue<string>("authority") ?? throw new ConfigurationErrorsException("Missing configuration value 'authority'");
            this.username = configuration.GetValue<string>("username") ?? throw new ConfigurationErrorsException("Missing configuration value 'username'");
            this.password = configuration.GetValue<string>("password") ?? throw new ConfigurationErrorsException("Missing configuration value 'password'");
            this.clientId = configuration.GetValue<string>("client_id") ?? throw new ConfigurationErrorsException("Missing configuration value 'client_id'");
            this.scope = configuration.GetValue<string>("scope") ?? throw new ConfigurationErrorsException("Missing configuration value 'scope'");
        }

        public string Authority { get; }

        public HttpMessageHandler Handler { get; private set; }

        public async Task InitializeAsync()
        {
            await this.fixture.InitializeAsync().ConfigureAwait(false);

            var automation = new BrowserAutomation(this.username, this.password);
            var browser = new Browser(automation);
            var options = new OidcClientOptions
            {
                Authority = this.Authority,
                ClientId = this.clientId,
                RedirectUri = $"http://127.0.0.1:{browser.Port}",
                Scope = this.scope,
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
        }

        public async Task DisposeAsync()
        {
            this.Handler?.Dispose();

            await this.fixture.DisposeAsync().ConfigureAwait(false);
        }

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