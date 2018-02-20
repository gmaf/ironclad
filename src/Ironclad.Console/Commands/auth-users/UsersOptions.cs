// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using Ironclad.Client;
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
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));
            app.Command("modify", command => ModifyUserCommand.Configure(command, options));
            app.Command("roles", command => AssignUserRolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                CommandName = "user",
                ArgumentName = "username",
                ArgumentDescription = "The username of the user.",
                DisplayCommand = (string value) => new ShowCommand.Display<User>(async context => await context.UsersClient.GetUserAsync(value).ConfigureAwait(false)),
                ListCommand = (string startsWith, int skip, int take) =>
                    new ShowCommand.List<UserSummary>(
                        "users",
                        async context => await context.UsersClient.GetUserSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("username", user => user.Username),
                        ("email", user => user.Email),
                        ("sub", user => user.Id)),
            };
    }
}