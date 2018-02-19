// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RemoveClientCommand : ICommand
    {
        private string clientId;

        private RemoveClientCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Unregisters the specified client";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RemoveClientCommand { clientId = argumentClientId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.ClientsClient.RemoveClientAsync(this.clientId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}