// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class UsersCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Provides user related operations";
            app.HelpOption();

            // commands
            ////app.Command("list", command => ListCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}
