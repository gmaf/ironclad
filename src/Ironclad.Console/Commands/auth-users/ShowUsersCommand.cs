// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal static class ShowUsersCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the users.";
            app.HelpOption();

            // arguments
            var argument = app.Argument("username", "The username of a user." /* You can use wildcards for searching."*/, false);

            // options
            var optionSkip = app.Option("-s|--skip", "The number of users to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of users to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argument.Value))
                    {
                        options.Command = new ShowCommand<User>(async context => await context.UsersClient.GetUserAsync(argument.Value).ConfigureAwait(false));
                        return;
                    }

                    var skip = 0;
                    if (optionSkip.HasValue() && !int.TryParse(optionSkip.Value(), out skip))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [skip] value of '{optionSkip.Value()}'");
                    }

                    var take = 20;
                    if (optionTake.HasValue() && !int.TryParse(optionTake.Value(), out take))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [take] value of '{optionTake.Value()}'");
                    }

                    options.Command = new ListCommand<UserSummary>(
                        "users",
                        async context => await context.UsersClient.GetUserSummariesAsync(skip, take).ConfigureAwait(false),
                        ("username", user => user.Username),
                        ("email",    user => user.Email),
                        ("sub",      user => user.Id));
                });
        }
    }
}
