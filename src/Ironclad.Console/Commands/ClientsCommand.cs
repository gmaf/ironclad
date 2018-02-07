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
            app.Command("list", command => ListCommand.Configure(command, options, console));
            app.Command("show", command => ShowCommand.Configure(command, options, console));
            app.Command("register", command => RegisterCommand.Configure(command, options, console));
            app.Command("scopes", command => ModifyScopesCommand.Configure(command, options, console));
            app.Command("enable", command => EnableCommand.Configure(command, options, console));
            app.Command("disable", command => DisableCommand.Configure(command, options, console));
            app.Command("token", command => ChangeTokenTypeCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}