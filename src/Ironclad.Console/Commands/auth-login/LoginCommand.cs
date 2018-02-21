// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using IdentityModel.OidcClient;
    using Ironclad.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class LoginCommand : ICommand
    {
        public const string DefaultAuthority = "https://auth.lykkecloud.com";

        private Api api;

        private LoginCommand()
        {
        }

        public string Authority { get; private set; }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Log in to an authorization server";

            // arguments
            var argumentAuthority = app.Argument("authority", "The URL for the authorization server to log in to");

            // options
            var optionTest = app.Option("-t|--test", "Uses the Lykke TEST authorization server", CommandOptionType.NoValue);
            var optionReset = app.Option("-r|--reset", "Resets the authorization context", CommandOptionType.NoValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(optionReset.Value()) && string.IsNullOrEmpty(argumentAuthority.Value) && string.IsNullOrEmpty(optionTest.Value()))
                    {
                        // only --reset was specified
                        options.Command = new Reset();
                        return;
                    }

                    var authority = argumentAuthority.Value;
                    if (string.IsNullOrEmpty(authority))
                    {
                        authority = string.IsNullOrEmpty(optionTest.Value()) ? DefaultAuthority : "https://auth-test.lykkecloud.com";
                    }
                    else if (!string.IsNullOrEmpty(optionTest.Value()))
                    {
                        ////console.WriteLine("Ignoring test option as authority was specified.");
                    }

                    // validate
                    if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri))
                    {
                        console.Error.WriteLine($"Invalid authority URL specified: {authority}.");
                        return;
                    }

                    var api = default(Api);
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            using (var response = client.GetAsync(new Uri(authority + "/api")).GetAwaiter().GetResult())
                            {
                                api = JsonConvert.DeserializeObject<Api>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            }
                        }
                        catch (HttpRequestException)
                        {
                            console.Error.WriteLine($"Unable to connect to: {authority}.");
                            return;
                        }
                    }

                    if (api == null)
                    {
                        console.Error.WriteLine($"Invalid response from: {authority}.");
                        return;
                    }

                    options.Command = new LoginCommand { Authority = authority, api = api };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            context.Console.WriteLine($"Logging in to {this.Authority} ({this.api.Title} v{this.api.Version} running on {this.api.OS})...");

            var data = context.Repository.GetCommandData();
            if (data != null && data.Authority == this.Authority)
            {
                // already logged in?
                var discoveryResponse = default(DiscoveryResponse);
                using (var discoveryClient = new DiscoveryClient(this.Authority))
                {
                    discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                    if (!discoveryResponse.IsError)
                    {
                        using (var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "auth_console"))
                        using (var refreshTokenHandler = new RefreshTokenHandler(tokenClient, data.RefreshToken, data.AccessToken))
                        using (var userInfoClient = new UserInfoClient(discoveryResponse.UserInfoEndpoint, refreshTokenHandler))
                        {
                            var response = await userInfoClient.GetAsync(data.AccessToken).ConfigureAwait(false);
                            if (!response.IsError)
                            {
                                var claimsIdentity = new ClaimsIdentity(response.Claims, "idSvr", "name", "role");
                                context.Console.WriteLine($"Logged in as {claimsIdentity.Name}.");
                                return;
                            }
                        }
                    }
                }
            }

            var browser = new SystemBrowser();
            var options = new OidcClientOptions
            {
                Authority = this.Authority,
                ClientId = "auth_console",
                RedirectUri = $"http://127.0.0.1:{browser.Port}",
                Scope = "openid profile email auth_api offline_access",
                FilterClaims = false,
                Browser = browser
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);
            if (result.IsError)
            {
                context.Console.Error.WriteLine($"Error attempting to log in:{Environment.NewLine}{result.Error}");
                return;
            }

            context.Repository.SetCommandData(
                new CommandData
                {
                    Authority = this.Authority,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                });

            context.Console.WriteLine($"Logged in as {result.User.Identity.Name}.");
        }

        public class Reset : ICommand
        {
            public Task ExecuteAsync(CommandContext context)
            {
                context.Repository.SetCommandData(null);
                return Task.CompletedTask;
            }
        }

#pragma warning disable CA1812
        private class Api
        {
            public string Title { get; set; }

            public string Version { get; set; }

            public string OS { get; set; }
        }
    }
}
