// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal static class ShowApiResourcesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the APIs.";
            app.HelpOption();

            // arguments
            var argument = app.Argument("name", "The name of the API." /* You can use wildcards for searching."*/, false);

            // options
            var optionSkip = app.Option("-s|--skip", "The number of APIs to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of APIs to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argument.Value))
                    {
                        options.Command = new ShowCommand<ApiResource>(
                            async context => await context.ApiResourcesClient.GetApiResourceAsync(argument.Value).ConfigureAwait(false));

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

                    options.Command = new ListCommand<ResourceSummary>(
                        "APIs",
                        async context => await context.ApiResourcesClient.GetApiResourceSummariesAsync(start: skip, size: take).ConfigureAwait(false),
                        ("name",        resource => resource.Name),
                        ("description", resource => resource.DisplayName),
                        ("enabled",     resource => resource.Enabled.ToString(CultureInfo.InvariantCulture)));
                });
        }
    }
}
