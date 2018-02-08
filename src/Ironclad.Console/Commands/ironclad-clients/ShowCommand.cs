// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowCommand : ICommand
    {
        private string clientId;

        private ShowCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the specified client";
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
            var client = await context.Client.GetClientAsync(this.clientId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(client, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}
