// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RemoveIdentityResourceCommand : ICommand
    {
        private string resourceName;

        private RemoveIdentityResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Removes the specified identity resource";
            app.HelpOption();

            // arguments
            var argumentResourceName = app.Argument("name", "The name of the resource to remove", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RemoveIdentityResourceCommand { resourceName = argumentResourceName.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.IdentityResourcesClient.RemoveIdentityResourceAsync(this.resourceName).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}