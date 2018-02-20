// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ApisOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = $"Provides API resources related operations";
            app.HelpOption();

            // commands
            app.Command("add", command => AddApiResourceCommand.Configure(command, options, console));
            app.Command("remove", command => RemoveCommand.Configure(command, options, GetRemoveCommandOptions()));
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static RemoveCommandOptions GetRemoveCommandOptions() =>
            new RemoveCommandOptions
            {
                Type = "API",
                ArgumentName = "name",
                ArgumentDescription = "The name of the API to remove.",
                RemoveCommand = value => new RemoveCommand(async context => await context.ApiResourcesClient.RemoveApiResourceAsync(value).ConfigureAwait(false)),
            };

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                Type = "API",
                ArgumentName = "name",
                ArgumentDescription = "The API name. You can end the API name with a wildcard to search.",
                DisplayCommand = (string value) =>
                    new ShowCommand.Display<ApiResource>(async context => await context.ApiResourcesClient.GetApiResourceAsync(value).ConfigureAwait(false)),
                ListCommand = (string startsWith, int skip, int take) =>
                    new ShowCommand.List<ResourceSummary>(
                        "APIs",
                        async context => await context.ApiResourcesClient.GetApiResourceSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("name", resource => resource.Name),
                        ("description", resource => resource.DisplayName),
                        ("enabled", resource => resource.Enabled.ToString(CultureInfo.InvariantCulture))),
            };
    }
}