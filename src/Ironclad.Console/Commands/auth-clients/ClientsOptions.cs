// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ClientsOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Manage clients";
            app.HelpOption();

            // commands
            app.Command("add", command => AddClientCommand.Configure(command, options, console));
            app.Command("remove", command => RemoveCommand.Configure(command, options, GetRemoveCommandOptions()));
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));
            app.Command("enable", command => EnableClientCommand.Configure(command, options));
            app.Command("disable", command => DisableClientCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static RemoveCommandOptions GetRemoveCommandOptions() =>
            new RemoveCommandOptions
            {
                Type = "client",
                ArgumentName = "id",
                ArgumentDescription = "The client identifier for the client to remove",
                RemoveCommand = value => new RemoveCommand(async context => await context.ClientsClient.RemoveClientAsync(value).ConfigureAwait(false)),
            };

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                Type = "client",
                ArgumentName = "id",
                ArgumentDescription = "The client identifier (you can end the client identifier with a wildcard to search)",
                DisplayCommand = (string value) => new ShowCommand.Display<Client>(async context => await context.ClientsClient.GetClientAsync(value).ConfigureAwait(false)),
                ListCommand = (string startsWith, int skip, int take) =>
                    new ShowCommand.List<ClientSummary>(
                        "clients",
                        async context => await context.ClientsClient.GetClientSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("id", client => client.Id),
                        ("name", client => client.Name),
                        ("enabled", client => client.Enabled.ToString(CultureInfo.InvariantCulture))),
            };
    }
}