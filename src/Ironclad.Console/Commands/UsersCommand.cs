// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class UsersCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides user related operations";
            app.HelpOption();

            // commands
            app.Command("list", command => Users.ListCommand.Configure(command, options));
            app.Command("show", command => Users.ShowCommand.Configure(command, options));
            app.Command("register", command => Users.RegisterCommand.Configure(command, options));
            app.Command("unregister", command => Users.UnregisterCommand.Configure(command, options));
            app.Command("modify", command => Users.ModifyCommand.Configure(command, options));
            app.Command("changepwd", command => Users.ChangePasswordCommand.Configure(command, options));
            app.Command("roles", command => ConfigureRoles(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }

        private static void ConfigureRoles(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides user roles related operations";
            app.HelpOption();

            app.Command("list", command => Users.ShowUserRolesCommand.Configure(command, options));
            app.Command("assign", command => Users.AssignRolesCommand.Configure(command, options));
            app.Command("unassign", command => Users.UnassignRolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}
