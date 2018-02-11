// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    public class AddApiResourceCommand : ICommand
    {
        private string name;
        private string displayName;
        private string apiSecret;
        private List<string> userClaims;
        private bool enabled;
        private List<ApiResource.Scope> apiScopes;

        private AddApiResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Adds the specified API resource";
            app.HelpOption();

            // arguments
            var argumentResourceName = app.Argument("name", "The resource name", false);

            // options
            var optionDisplayName = app.Option("--displayName <name>", "The resource displayName.", CommandOptionType.SingleValue);
            var optionApiSecret = app.Option("--apiSecret <secret>", "The resource apiSecret.", CommandOptionType.SingleValue);
            var optionUserClaim = app.Option("--userClaim <claim>", "The resource userClaim. You can call this several times.", CommandOptionType.MultipleValue);
            var optionEnabled = app.Option("--enabled", "Is the resource enabled.", CommandOptionType.NoValue);
            var optionApiScope = app.Option("--apiScope <name:claim1,claim2...>", "The resource apiScope. You can call this several times.", CommandOptionType.MultipleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    var apiScopes = new List<ApiResource.Scope>();
                    foreach (var apiScope in optionApiScope.Values)
                    {
                        var name2claims = apiScope.Split(':');
                        if (name2claims.Length != 2)
                        {
                            throw new ArgumentException($"Malformed API scope '{apiScope}'.");
                        }

                        apiScopes.Add(
                            new ApiResource.Scope
                            {
                                Name = name2claims[0],
                                UserClaims = name2claims[1].Split(',')
                            });
                    }

                    options.Command = new AddApiResourceCommand
                    {
                        name = argumentResourceName.Value,
                        displayName = optionDisplayName.Value(),
                        apiSecret = optionApiSecret.Value(),
                        userClaims = optionUserClaim.Values,
                        enabled = optionEnabled.HasValue(),
                        apiScopes = apiScopes,
                    };
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
                Enabled = this.enabled,
                ApiScopes = this.apiScopes,
            };

            await context.ApiResourcesClient.AddApiResourceAsync(resource).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}