// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddScopeCommand : ICommand
    {
        private string displayName;
        private string name;
        private bool? enabled;
        private ICollection<string> userClaims;

        private AddScopeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Creates a new identity-based scope";

            // arguments
            var argumentName = app.Argument("name", "The name of the identity-based scope", false);

            // options
#pragma warning disable SA1025
            var optionDisplayName =                 app.Option("-n|--display_name <name>",         "The display name of the identity-based scope",                                                    CommandOptionType.SingleValue);
            var optionUserClaims =                  app.Option("-u|--user_claims <user_claim>",    "The user claim types associated with the identity-based scope (you can call this several times)", CommandOptionType.MultipleValue);
            var optionDisabled =                    app.Option("-d|--disabled",                    "Creates the new identity-based scope in a disabled state",                                        CommandOptionType.NoValue);
#pragma warning restore SA1025

            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentName.Value))
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    options.Command = new AddScopeCommand
                    {
                        displayName = string.IsNullOrEmpty(optionDisplayName.Value())
                            ? argumentName.Value
                            : optionDisplayName.Value(),
                        name = argumentName.Value,
                        userClaims =
                            optionUserClaims.HasValue() ? optionUserClaims.Values.Distinct().ToHashSet() : null,
                        enabled = optionDisabled.HasValue() ? (bool?) false : null
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.IdentityResourcesClient
            .AddIdentityResourceAsync(new IdentityResource
            {
                DisplayName = this.displayName,
                Name = this.name,
                UserClaims = this.userClaims,
                Enabled = this.enabled
            }).ConfigureAwait(false);
    }
}
