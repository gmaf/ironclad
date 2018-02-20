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
            app.Command("show", command => CommonShowCommand.Configure(command, options, new UsersShowTraits()));
            app.Command("modify", command => ModifyUserCommand.Configure(command, options));
            app.Command("roles", command => AssignUserRolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private class UsersShowTraits : IShowTraits
        {
            public string Name => "user";

            public string ArgumentName => "username";

            public string ArgumentDescription => "The username of the user.";

            public ICommand GetShowCommand(string value) =>
                new ShowCommand<User>(async context => await context.UsersClient.GetUserAsync(value).ConfigureAwait(false));

            public ICommand GetListCommand(string startsWith, int skip, int take) => new ListCommand<UserSummary>(
                "users",
                async context => await context.UsersClient.GetUserSummariesAsync(start: skip, size: take).ConfigureAwait(false),
                ("username", user => user.Username),
                ("email", user => user.Email),
                ("sub", user => user.Id));
        }
    }
}