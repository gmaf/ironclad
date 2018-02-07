// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Ironclad.Console.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Ironclad.Console.Commands
{
    internal class EnableCommand : ICommand
    {
        private string clientId;

        private EnableCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Enable the specified client";
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

                    options.Command = new EnableCommand
                    {
                        clientId = argumentClientId.Value
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
                Enabled = true
            };

            await context.Client.ModifyClientAsync(client).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}