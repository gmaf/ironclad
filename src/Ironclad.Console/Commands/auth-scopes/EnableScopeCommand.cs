// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class EnableScopeCommand : ICommand
    {
        private string name;

        private EnableScopeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Enable an identity-based scope";
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

                    options.Command = new EnableScopeCommand
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
                Enabled = true
            };

            await context.IdentityResourcesClient.ModifyIdentityResourceAsync(identityResource).ConfigureAwait(false);
        }
    }
}