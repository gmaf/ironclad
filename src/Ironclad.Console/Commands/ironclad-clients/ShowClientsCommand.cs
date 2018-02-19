// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowClientsCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Shows the clients";
            app.HelpOption();

            // options
            var optionSkip = app.Option("-s|--skip", "The number of clients to skip", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of clients to take", CommandOptionType.SingleValue);

            // arguments
            var argumentClientId = app.Argument("id", "The ID of the client to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        options.Command = new ShowCommand { ClientId = argumentClientId.Value };
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

                    options.Command = new ListCommand { Skip = skip, Take = take };
                });
        }

        private class ListCommand : ICommand
        {
            public int Skip { get; set; }

            public int Take { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var clients = await context.ClientsClient.GetClientSummariesAsync(this.Skip, this.Take).ConfigureAwait(false);
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

        private class ShowCommand : ICommand
        {
            public string ClientId { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var client = await context.ClientsClient.GetClientAsync(this.ClientId).ConfigureAwait(false);
                await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(client)).ConfigureAwait(false);
            }
        }
    }
}
