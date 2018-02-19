// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class UsersOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides user related operations";
            app.HelpOption();

            // commands
            app.Command("add", command => AddUserCommand.Configure(command, options));
            app.Command("show", command => ShowUsersCommand.Configure(command, options));
            app.Command("modify", command => ModifyUserCommand.Configure(command, options));
            app.Command("roles", command => AssignUserRolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }
    }
}
