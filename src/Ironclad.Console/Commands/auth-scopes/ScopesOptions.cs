// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Client;
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ScopesOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Manage scopes";
            app.HelpOption();

            // commands
            app.Command("add", command => AddScopeCommand.Configure(command, options, console));
            app.Command("modify", command => ModifyScopeCommand.Configure(command, options, console));
            app.Command("remove", command => RemoveCommand.Configure(command, options, GetRemoveCommandOptions()));
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));
            app.Command("enable", command => EnableScopeCommand.Configure(command, options));
            app.Command("disable", command => DisableScopeCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static RemoveCommandOptions GetRemoveCommandOptions() =>
            new RemoveCommandOptions
            {
                Type = "scope",
                ArgumentName = "scope",
                ArgumentDescription = "The identity-based scope to remove",
                RemoveCommand = value => new RemoveCommand(async context => await context.IdentityResourcesClient.RemoveIdentityResourceAsync(value).ConfigureAwait(false))
            };

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                Type = "scope",
                ArgumentName = "scope",
                ArgumentDescription = "The identity-based scope (you can end the identity-based scope with a wildcard to search)",
                DisplayCommand = (string value) => new ShowCommand.Display<IdentityResource>(async context => await context.IdentityResourcesClient.GetIdentityResourceAsync(value).ConfigureAwait(false)),
                ListCommand = (string startsWith, int skip, int take) =>
                    new ShowCommand.List<ResourceSummary>(
                        "scopes",
                        async context => await context.IdentityResourcesClient.GetIdentityResourceSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("name", scope => scope.Name),
                        ("display_name", scope => scope.DisplayName),
                        ("enabled", scope => scope.Enabled.ToString(CultureInfo.InvariantCulture)))
            };
    }
}