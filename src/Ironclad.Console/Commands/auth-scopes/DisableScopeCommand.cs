// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class DisableScopeCommand : ICommand
    {
        private string name;

        private DisableScopeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Disables an identity-based scope";
            app.HelpOption();

            // arguments
            var argumentName = app.Argument("name", "The name of the identity-based scope", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new DisableScopeCommand
                    {
                        name = argumentName.Value
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var identityResource = new Client.IdentityResource
            {
                Name = this.name,
                UserClaims = null,
                Enabled = false
            };

            await context.IdentityResourcesClient.ModifyIdentityResourceAsync(identityResource).ConfigureAwait(false);
        }
    }
}