// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ClientsCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Provides client related operations";
            app.HelpOption();

            // commands
            app.Command("show", command => ShowClientsCommand.Configure(command, options));
            app.Command("add", command => AddClientCommand.Configure(command, options));
            app.Command("remove", command => RemoveClientCommand.Configure(command, options));
            app.Command("scopes", command => ModifyClientScopesCommand.Configure(command, options));
            app.Command("enable", command => EnableClientCommand.Configure(command, options));
            app.Command("disable", command => DisableClientCommand.Configure(command, options));
            app.Command("token", command => ChangeClientTokenTypeCommand.Configure(command, options));
            app.Command("uris", command => UpdateClientUrisCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }
    }
}