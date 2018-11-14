// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class EnableIdentityCommand : ICommand
    {
        private string name;

        private EnableIdentityCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Enable an identity resource";
            app.HelpOption();

            // arguments
            var argumentName = app.Argument("name", "The name of the identity resource", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new EnableIdentityCommand
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
                Enabled = true
            };

            await context.IdentityResourcesClient.ModifyIdentityResourceAsync(identityResource).ConfigureAwait(false);
        }
    }
}