// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Roles
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
            app.Description = "Lists the registered roles";
            app.HelpOption();

            // options
            var optionSkip = app.Option("-s|--skip", "The number of roles to skip", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of roles to take", CommandOptionType.SingleValue);

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
            var roles = await context.Client.GetRoleSummariesAsync(this.skip, this.take).ConfigureAwait(false);
            var maxRoleIdLength = roles.Max(c => c.Id?.Length ?? 0);
            var outputFormat = string.Format(CultureInfo.InvariantCulture, "  {{0, -{0}}}{{1}}", maxRoleIdLength + 2);

            await context.Console.Out.WriteLineAsync("Roles:").ConfigureAwait(false);

            foreach (var user in roles)
            {
                context.Console.Out.WriteLine(outputFormat, user.Id, user.Name);
            }

            await context.Console.Out.WriteLineAsync($"Showing from {roles.Start + 1} to {roles.Start + roles.Size} of {roles.TotalSize} in total.")
                .ConfigureAwait(false);
        }
    }
}
