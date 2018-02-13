// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddClientCommand : ICommand
    {
        private Ironclad.Client.Client client;

        private AddClientCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Creates a new client trust relationship with the auth server";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("type", "The type of client to add. Allowed values are s[erver], w[ebsite], and c[onsole].", false);

            // options
            var optionsName = app.Option("-i|--id", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-s|--secret", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);
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

        public async Task ExecuteAsync(CommandContext context) => await context.ClientsClient.RegisterClientAsync(client).ConfigureAwait(false);
    }
}
