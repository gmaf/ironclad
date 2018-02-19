// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal static class ShowUsersCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the users.";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username of a user. You can use wildcards for searching.", false);

            // options
            var optionSkip = app.Option("-s|--skip", "The number of users to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of users to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argumentUsername.Value))
                    {
                        options.Command = new ShowCommand { Username = argumentUsername.Value };
                        return;
                    }

                    var skip = 0;
                    if (optionSkip.HasValue() && !int.TryParse(optionSkip.Value(), out skip))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [skip] value of '{optionSkip.Value()}'");
                    }

                    var take = 20;
                    if (optionTake.HasValue() && !int.TryParse(optionTake.Value(), out skip))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [take] value of '{optionTake.Value()}'");
                    }

                    options.Command = new ListCommand { Skip = skip, Take = take };
                });
        }

        private class ListCommand : ICommand
        {
            public int Skip { get; set; }

            public int Take { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var users = await context.UsersClient.GetUserSummariesAsync(this.Skip, this.Take).ConfigureAwait(false);

                var maxUsernameLength = users.Max(c => c.Username?.Length ?? 0);
                var maxUserEmailLength = users.Max(c => c.Email?.Length ?? 0);

                var outputFormat = string.Format(CultureInfo.InvariantCulture, " {{0, -{0}}}{{1, -{1}}}{{2}}", maxUsernameLength + 2, maxUserEmailLength + 2);

                await context.Console.Out.WriteLineAsync().ConfigureAwait(false);

                context.Console.Out.WriteLine(outputFormat, "username", "email", "sub");
                foreach (var user in users)
                {
                    context.Console.Out.WriteLine(outputFormat, user.Username, user.Email, user.Id);
                }

                await context.Console.Out.WriteLineAsync().ConfigureAwait(false);
                await context.Console.Out.WriteLineAsync($"Showing from {users.Start + 1} to {users.Start + users.Size} of {users.TotalSize} users in total.")
                    .ConfigureAwait(false);
            }
        }

        private class ShowCommand : ICommand
        {
            public string Username { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var user = await context.UsersClient.GetUserAsync(this.Username).ConfigureAwait(false);
                await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(user)).ConfigureAwait(false);
            }
        }
    }
}
