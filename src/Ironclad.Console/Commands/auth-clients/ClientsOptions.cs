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
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IReporter reporter)
        {
            // description
            app.Description = "Provides client related operations";
            app.HelpOption();

            // commands
            app.Command("add", command => AddClientCommand.Configure(command, options, reporter));
            app.Command("remove", command => RemoveClientCommand.Configure(command, options));
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));
            app.Command("scopes", command => ModifyClientScopesCommand.Configure(command, options));
            app.Command("enable", command => EnableClientCommand.Configure(command, options));
            app.Command("disable", command => DisableClientCommand.Configure(command, options));
            app.Command("token", command => ChangeClientTokenTypeCommand.Configure(command, options));
            app.Command("uris", command => UpdateClientUrisCommand.Configure(command, options, reporter));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                CommandName = "client",
                ArgumentName = "id",
                ArgumentDescription = "The client identifier.",
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