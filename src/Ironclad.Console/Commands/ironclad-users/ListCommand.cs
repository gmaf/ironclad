// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ListCommand : ICommand
    {
        private int skip;
        private int take;

        private ListCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the registered users";
            app.HelpOption();

            // options
            var optionSkip = app.Option("-s|--skip", "The number of users to skip", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of users to take", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
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

                    options.Command = new ListCommand { skip = skip, take = take };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var users = await context.Client.GetUserSummariesAsync(this.skip, this.take).ConfigureAwait(false);
            var maxUserIdLength = users.Max(c => c.Id?.Length ?? 0);
            var outputFormat = string.Format(CultureInfo.InvariantCulture, "  {{0, -{0}}}{{1}}", maxUserIdLength + 2);

            await context.Console.Out.WriteLineAsync("Users:").ConfigureAwait(false);

            foreach (var user in users)
            {
                context.Console.Out.WriteLine(outputFormat, user.Id, user.Username);
            }

            await context.Console.Out.WriteLineAsync($"Showing from {users.Start + 1} to {users.Start + users.Size} of {users.TotalSize} in total.")
                .ConfigureAwait(false);
        }
    }
}
