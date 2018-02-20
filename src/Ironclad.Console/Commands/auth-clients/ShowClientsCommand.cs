// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal static class ShowClientsCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the clients.";
            app.HelpOption();

            // arguments
            var argument = app.Argument("id", "The client ID." /* You can use wildcards for searching."*/, false);

            // options
            var optionSkip = app.Option("-s|--skip", "The number of clients to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of clients to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argument.Value))
                    {
                        options.Command = new ShowCommand<Client>(async context => await context.ClientsClient.GetClientAsync(argument.Value).ConfigureAwait(false));

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

                    options.Command = new ListCommand<ClientSummary>(
                        "clients",
                        async context => await context.ClientsClient.GetClientSummariesAsync(start: skip, size: take).ConfigureAwait(false),
                        ("id",      client => client.Id),
                        ("name",    client => client.Name),
                        ("enabled", client => client.Enabled.ToString(CultureInfo.InvariantCulture)));
                });
        }
    }
}
