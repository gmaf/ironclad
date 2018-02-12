// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    public class ListIdentityResourcesCommand : ICommand
    {
        private int skip;
        private int take;

        private ListIdentityResourcesCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Lists the identity resources";
            app.HelpOption();

            // options
            var optionSkip = app.Option("-s|--skip", "The number of clients to skip", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of clients to take", CommandOptionType.SingleValue);

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

                    options.Command = new ListIdentityResourcesCommand { skip = skip, take = take };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var resources = await context.IdentityResourcesClient.GetIdentityResourceSummariesAsync(this.skip, this.take).ConfigureAwait(false);
            var maxResourceIdLength = resources.Max(c => c.Name?.Length ?? 0);
            var outputFormat = string.Format(CultureInfo.InvariantCulture, "  {{0, -{0}}}{{1}}", maxResourceIdLength + 2);

            await context.Console.Out.WriteLineAsync("Identity Resources:").ConfigureAwait(false);

            foreach (var resource in resources)
            {
                context.Console.Out.WriteLine(outputFormat, resource.Name, resource.DisplayName);
            }

            await context.Console.Out
                .WriteLineAsync($"Showing from {resources.Start + 1} to {resources.Start + resources.Size} of {resources.TotalSize} in total.")
                .ConfigureAwait(false);
        }
    }
}