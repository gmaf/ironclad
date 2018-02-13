// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class NewClientCommand : ICommand
    {
        private string clientId;
        private string clientSecret;
        private string clientName;

        private NewClientCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Creates a new client trust relationship with the auth server";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);
            var argumentClientSecret = app.Argument("secret", "The client secret", false);

            // options
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value) || string.IsNullOrEmpty(argumentClientSecret.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new NewClientCommand
                    {
                        clientId = argumentClientId.Value,
                        clientSecret = argumentClientSecret.Value,
                        clientName = optionsName.Value()
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
                Name = this.clientName,
                Secret = this.clientSecret,
            };

            await context.ClientsClient.RegisterClientAsync(client).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
