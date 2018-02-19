// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    public class AddApiResourceCommand : ICommand
    {
        private string name;
        private string displayName;
        private string apiSecret;
        private List<string> userClaims;
        private List<ApiResource.Scope> apiScopes;

        private AddApiResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = $"Adds the specified web API.";

            app.ExtendedHelpText = $"{Environment.NewLine}Use \"apis add\" without argument to enter in interactive mode.{Environment.NewLine}";

            // arguments
            var argumentResourceName = app.Argument("name", "The web API name.", false);

            // options
            var optionDisplayName = app.Option("-d|--description <description>", "The web API description.", CommandOptionType.SingleValue);
            var optionApiSecret = app.Option("-s|--secret <secret>", "The web API secret.", CommandOptionType.SingleValue);
            var optionUserClaim = app.Option(
                "-c|--claim <claim>",
                "A web API user claim. You can call this several times.",
                CommandOptionType.MultipleValue);
            var optionApiScope = app.Option(
                "-a|--apiscope <name:claim1,claim2...>",
                "The web API scope. You can call this several times.",
                CommandOptionType.MultipleValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        options.Command = GetApiFromPrompt(console);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(optionApiSecret.Value()))
                        {
                            LogError(console, "secret cannot be null.");
                            return;
                        }

                        var apiScopes = new List<ApiResource.Scope>();
                        foreach (var apiScope in optionApiScope.Values)
                        {
                            var name2claims = apiScope.Split(':');
                            var scope = new ApiResource.Scope
                            {
                                Name = name2claims[0],
                                UserClaims = name2claims.Length != 2 ? null : name2claims[1].Split(',')
                            };

                            apiScopes.Add(scope);
                        }

                        options.Command = new AddApiResourceCommand
                        {
                            name = argumentResourceName.Value,
                            displayName = optionDisplayName.Value(),
                            apiSecret = optionApiSecret.Value(),
                            userClaims = optionUserClaim.HasValue() ? optionUserClaim.Values : null,
                            apiScopes = apiScopes.Any() ? apiScopes : null,
                        };
                    }
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var resource = new ApiResource
            {
                Name = this.name,
                DisplayName = this.displayName,
                ApiSecret = this.apiSecret,
                UserClaims = this.userClaims,
                ApiScopes = this.apiScopes,
            };

            await context.ApiResourcesClient.AddApiResourceAsync(resource).ConfigureAwait(false);
        }

        private static void LogInfo(IConsole console, string msg)
        {
            console.WriteLine(msg);
        }

        private static void LogError(IConsole console, string msg)
        {
            console.ForegroundColor = ConsoleColor.Red;
            console.Error.WriteLine(msg);
            console.ResetColor();
        }

        private static AddApiResourceCommand GetApiFromPrompt(IConsole console)
        {
            var name = Prompt.GetString("Name:");
            if (string.IsNullOrEmpty(name))
            {
                LogError(console, "Name cannot be null");
                return null;
            }

            var displayName = Prompt.GetString("Display Name:", name);
            var secret = Prompt.GetPassword("Secret:");
            if (string.IsNullOrEmpty(secret))
            {
                LogError(console, "Secret cannot be null");
                return null;
            }

            var rawClaims = Prompt.GetString("Claims (claim1,claim2,...):");
            var claims = rawClaims?.Split(",")?.ToList() ?? new List<string>();

            LogInfo(console, "Scopes part. Leave empty once we want no more scopes.");
            var scopes = new List<ApiResource.Scope>();
            while (true)
            {
                var scopeName = Prompt.GetString($"Scope {scopes.Count + 1}:");
                if (string.IsNullOrEmpty(scopeName))
                {
                    break;
                }

                var scopeClaims = Prompt.GetString($"{scopeName} Claims (claim1,claim2,...):", rawClaims)?.Split(",")?.ToList() ?? new List<string>();

                scopes.Add(
                    new ApiResource.Scope
                    {
                        Name = scopeName,
                        UserClaims = scopeClaims
                    });
            }

            return new AddApiResourceCommand
            {
                name = name,
                displayName = displayName,
                apiSecret = secret,
                userClaims = claims,
                apiScopes = scopes
            };
        }
    }
}