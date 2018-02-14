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

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Adds the specified web API.";

            // arguments
            var argumentResourceName = app.Argument("name", "The web API name.", false);

            // options
            var optionDisplayName = app.Option("-d|--description <description>", "The web API description.", CommandOptionType.SingleValue);
            var optionApiSecret = app.Option("-s|--secret <secret>", "The web API secret.", CommandOptionType.SingleValue, o => o.IsRequired(false));
            var optionUserClaim = app.Option("-c|--claim <claim>", "A web API user claim. You can call this several times.", CommandOptionType.MultipleValue);
            var optionApiScope = app.Option("-a|--apiscope <name:claim1,claim2...>", "The web API scope. You can call this several times.", CommandOptionType.MultipleValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        app.ShowVersionAndHelp();
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
                        userClaims = optionUserClaim.HasValue() ? optionUserClaim.Values : null,
                        apiScopes = apiScopes.Any() ? apiScopes : null,
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
                ApiScopes = this.apiScopes,
            };

            await context.ApiResourcesClient.AddApiResourceAsync(resource).ConfigureAwait(false);
        }
    }
}