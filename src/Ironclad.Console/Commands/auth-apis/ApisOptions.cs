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
            app.Command("show", command => CommonShowCommand.Configure(command, options, new ApisShowTraits()));
            app.Command("remove", command => RemoveApiResourceCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private class ApisShowTraits : IShowTraits
        {
            public string Name => "API";

            public string ArgumentName => "name";

            public string ArgumentDescription => "The name of the API.";

            public ICommand GetShowCommand(string value) =>
                new ShowCommand<ApiResource>(async context => await context.ApiResourcesClient.GetApiResourceAsync(value).ConfigureAwait(false));

            public ICommand GetListCommand(string startsWith, int skip, int take) => new ListCommand<ResourceSummary>(
                "APIs",
                async context => await context.ApiResourcesClient.GetApiResourceSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                ("name", resource => resource.Name),
                ("description", resource => resource.DisplayName),
                ("enabled", resource => resource.Enabled.ToString(CultureInfo.InvariantCulture)));
        }
    }
}