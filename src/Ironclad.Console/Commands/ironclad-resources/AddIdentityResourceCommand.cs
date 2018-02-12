// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    public class AddIdentityResourceCommand : ICommand
    {
        private string name;
        private string displayName;
        private List<string> userClaims;
        private bool enabled;

        private AddIdentityResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Adds the specified identity resource";
            app.HelpOption();

            // arguments
            var argumentResourceName = app.Argument("name", "The resource name", false);

            // options
            var optionDisplayName = app.Option("--displayName <name>", "The resource display name.", CommandOptionType.SingleValue);
            var optionUserClaim = app.Option("--userClaim <claim>", "The resource user claim. You can call this several times.", CommandOptionType.MultipleValue);
            var optionEnabled = app.Option("--enabled", "Is the resource enabled.", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AddIdentityResourceCommand
                    {
                        name = argumentResourceName.Value,
                        displayName = optionDisplayName.Value(),
                        userClaims = optionUserClaim.Values,
                        enabled = optionEnabled.HasValue(),
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var resource = new IdentityResource
            {
                Name = this.name,
                DisplayName = this.displayName,
                UserClaims = this.userClaims,
                Enabled = this.enabled,
            };

            await context.IdentityResourcesClient.AddIdentityResourceAsync(resource).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}