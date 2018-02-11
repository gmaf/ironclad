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
            app.Command("list", command => ListUsersCommand.Configure(command, options));
            app.Command("show", command => ShowUserCommand.Configure(command, options));
            app.Command("register", command => RegisterUserCommand.Configure(command, options));
            ////app.Command("unregister", command => UnregisterCommand.Configure(command, options));
            app.Command("modify", command => ModifyUserCommand.Configure(command, options));
            ////app.Command("changepwd", command => ChangePasswordCommand.Configure(command, options));
            app.Command("roles", command => AssignUserRolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}
