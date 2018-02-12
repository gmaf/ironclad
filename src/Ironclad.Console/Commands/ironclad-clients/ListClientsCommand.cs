// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ListClientsCommand : ICommand
    {
        private int skip;
        private int take;

        private ListClientsCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the registered clients";
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

                    options.Command = new ListClientsCommand { skip = skip, take = take };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var clients = await context.ClientsClient.GetClientSummariesAsync(this.skip, this.take).ConfigureAwait(false);
            var maxClientIdLength = clients.Max(c => c.Id?.Length ?? 0);
            var outputFormat = string.Format(CultureInfo.InvariantCulture, "  {{0, -{0}}}{{1}}", maxClientIdLength + 2);

            await context.Console.Out.WriteLineAsync("Clients:").ConfigureAwait(false);

            foreach (var client in clients)
            {
                context.Console.Out.WriteLine(outputFormat, client.Id, client.Name);
            }

            await context.Console.Out.WriteLineAsync($"Showing from {clients.Start + 1} to {clients.Start + clients.Size} of {clients.TotalSize} in total.")
                .ConfigureAwait(false);
        }
    }
}
