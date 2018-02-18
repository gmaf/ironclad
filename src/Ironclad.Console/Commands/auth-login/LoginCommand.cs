// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.OidcClient;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class LoginCommand : ICommand
    {
        public const string DefaultAuthority = "https://auth.lykkecloud.com";

        private string authority;
        private Api api;

        private LoginCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Logs in to the specified authorization server";

            // arguments
            var argumentAuthority = app.Argument("authority", "The URL for the authorization server to log in to");

            // options
            var optionTest = app.Option("-t|--test", "Uses the Lykke TEST authorization server", CommandOptionType.NoValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
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

                    options.Command = new LoginCommand { authority = authority, api = api };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            context.Console.WriteLine($"Logging in to {this.authority} ({this.api.Title} v{this.api.Version} running on {this.api.OS})...");

            var browser = new SystemBrowser();
            var options = new OidcClientOptions
            {
                Authority = this.authority,
                ClientId = "auth_console",
                RedirectUri = $"http://127.0.0.1:{browser.Port}",
                Scope = "openid email auth_api offline_access",
                FilterClaims = false,
                Browser = browser
            };

            var oidcClient = new OidcClient(options);
            var result = await oidcClient.LoginAsync(new LoginRequest()).ConfigureAwait(false);
            if (result.IsError)
            {
                context.Console.Error.WriteLine($"Error attempting to log in:\r\n{result.Error}");
            }

            context.Console.WriteLine($"token: {result.AccessToken}");
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
