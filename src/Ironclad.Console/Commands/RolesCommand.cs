// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class RolesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides role related operations";
            app.HelpOption();

            // commands
            app.Command("list", command => Roles.ListCommand.Configure(command, options));
            app.Command("show", command => Roles.ShowCommand.Configure(command, options));
            app.Command("register", command => Roles.RegisterCommand.Configure(command, options));
            app.Command("unregister", command => Roles.UnregisterCommand.Configure(command, options));
            app.Command("modify", command => Roles.ModifyCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}
