// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ChangeTokenTypeCommand : ICommand
    {
        private string clientId;
        private string clientTokenType;

        private ChangeTokenTypeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Change the token type of the specified client";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);
            var argumentClientTokenType = app.Argument("tokenType", "The client token type", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value) ||
                        string.IsNullOrEmpty(argumentClientTokenType.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ChangeTokenTypeCommand
                    {
                        clientId = argumentClientId.Value,
                        clientTokenType = argumentClientTokenType.Value
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
                AccessTokenType = this.clientTokenType
            };

            await context.Client.ModifyClientAsync(client).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}