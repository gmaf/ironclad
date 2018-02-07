// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using Ironclad.Console.Sdk;
    using Microsoft.Extensions.CommandLineUtils;

    internal class ShowCommand : ICommand
    {
        private string clientId;

        private ShowCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Lists the registered clients";
            app.Syntax = "Syntax";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("clientId", "The client ID to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowCommand { clientId = argumentClientId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Console.Out.WriteLineAsync($"Show {this.clientId}").ConfigureAwait(false);
        }
    }
}
