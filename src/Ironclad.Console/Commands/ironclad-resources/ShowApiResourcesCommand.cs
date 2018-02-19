// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowApiResourcesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Shows the API resources";
            app.HelpOption();

            // options
            var optionSkip = app.Option("-s|--skip", "The number of API resources to skip", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of API resources to take", CommandOptionType.SingleValue);

            // arguments
            var argumentResourceName = app.Argument("name", "The name of the API resource to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argumentResourceName.Value))
                    {
                        options.Command = new ShowCommand { ResourceName = argumentResourceName.Value };
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
                var resources = await context.ApiResourcesClient.GetApiResourceSummariesAsync(this.Skip, this.Take).ConfigureAwait(false);
                var maxResourceIdLength = resources.Max(c => c.Name?.Length ?? 0);
                var outputFormat = string.Format(CultureInfo.InvariantCulture, "  {{0, -{0}}}{{1}}", maxResourceIdLength + 2);

                await context.Console.Out.WriteLineAsync("API Resources:").ConfigureAwait(false);

                foreach (var resource in resources)
                {
                    context.Console.Out.WriteLine(outputFormat, resource.Name, resource.DisplayName);
                }

                await context.Console.Out
                    .WriteLineAsync($"Showing from {resources.Start + 1} to {resources.Start + resources.Size} of {resources.TotalSize} in total.")
                    .ConfigureAwait(false);
            }
        }

        private class ShowCommand : ICommand
        {
            public string ResourceName { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var resource = await context.ApiResourcesClient.GetApiResourceAsync(this.ResourceName).ConfigureAwait(false);
                await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(resource)).ConfigureAwait(false);
            }
        }
    }
}
