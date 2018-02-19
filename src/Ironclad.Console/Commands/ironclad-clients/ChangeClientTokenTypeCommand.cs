// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ChangeClientTokenTypeCommand : ICommand
    {
        private string clientId;
        private string accessTokenType;

        private ChangeClientTokenTypeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Change the access token type for the specified client";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);
            var argumentClientTokenType = app.Argument("tokenType", "The access token type", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value) || string.IsNullOrEmpty(argumentClientTokenType.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ChangeClientTokenTypeCommand
                    {
                        clientId = argumentClientId.Value,
                        accessTokenType = argumentClientTokenType.Value
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
                AccessTokenType = this.accessTokenType
            };

            await context.ClientsClient.ModifyClientAsync(client).ConfigureAwait(false);
        }
    }
}