// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.OidcClient;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Npgsql;

    public sealed class IroncladFixture : IDisposable
    {
        private const string ConnectionString = "Host=localhost;Database=ironclad;Username=postgres;Password=postgres;";

        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process postgresProcess;
        private readonly Process ironcladProcess;

        public IroncladFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.Authority = config.GetValue<string>("authority");

            this.postgresProcess = this.StartPostgres();
            this.ironcladProcess = this.StartIronclad();

            this.Handler = this.CreateTokenHandler().GetAwaiter().GetResult();
        }

        public string Authority { get; }

        public HttpMessageHandler Handler { get; }

        public void Dispose()
        {
            try
            {
                this.ironcladProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.ironcladProcess.Dispose();

            try
            {
                this.postgresProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.postgresProcess.Dispose();

            // NOTE (Cameron): Remove the docker container.
            Process.Start(new ProcessStartInfo("docker", $"stop {DockerContainerId}"))
                .WaitForExit(10000);
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        [DebuggerStepThrough]
        private Process StartPostgres()
        {
            var process = Process.Start(
                new ProcessStartInfo("docker", $"run --rm --name {DockerContainerId} -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ironclad -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            // NOTE (Cameron): Trying to find a sensible value here so as to not throw during a debug session.
            Thread.Sleep(3000);

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        connection.Open();
                        break;
                    }
                    catch (Exception ex) when (ex is NpgsqlException || ex is SocketException || ex is EndOfStreamException)
                    {
                        if (++attempt >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return process;
        }

        [DebuggerStepThrough]
        private Process StartIronclad()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}Ironclad{0}Ironclad.csproj",
                Path.DirectorySeparatorChar);

            Process.Start(
                new ProcessStartInfo("dotnet", $"run -p {path} --connectionString '{ConnectionString}'")
                {
                    UseShellExecute = true,
                });

            var processId = default(int);
            using (var client = new HttpClient())
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        using (var response = client.GetAsync(new Uri(this.Authority + "/api")).GetAwaiter().GetResult())
                        {
                            var api = JsonConvert.DeserializeObject<IroncladApi>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), GetJsonSerializerSettings());
                            processId = int.Parse(api.ProcessId, CultureInfo.InvariantCulture);
                        }

                        break;
                    }
                    catch (HttpRequestException)
                    {
                        if (++attempt >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return Process.GetProcessById(processId);
        }

        private async Task<HttpMessageHandler> CreateTokenHandler()
        {
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
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);

            return new TokenHandler(result.AccessToken);
        }

        private sealed class TokenHandler : DelegatingHandler
        {
            private string accessToken;

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

#pragma warning disable CA1812
        private class IroncladApi
        {
            public string ProcessId { get; set; }
        }
    }
}