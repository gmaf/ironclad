// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RemoveApiResourceCommand : ICommand
    {
        private string resourceName;

        private RemoveApiResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Removes the specified API resource";
            app.HelpOption();

            // arguments
            var argumentResourceName = app.Argument("resourceName", "The resource name to delete", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RemoveApiResourceCommand { resourceName = argumentResourceName.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.ApiResourcesClient.RemoveApiResourceAsync(this.resourceName).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}