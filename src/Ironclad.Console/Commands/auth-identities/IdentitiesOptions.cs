// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Ironclad.Client;

namespace Ironclad.Console.Commands
{
    using System;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class IdentitiesOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Manage identities";
            app.HelpOption();

            // commands
            app.Command("add", command => AddIdentityCommand.Configure(command, options));
            app.Command("modify", command => ModifyIdentityCommand.Configure(command, options));
            app.Command("remove", command => RemoveCommand.Configure(command, options, GetRemoveCommandOptions()));
            app.Command("show", command => ShowCommand.Configure(command, options, GetShowCommandOptions()));
            app.Command("enable", command => EnableIdentityCommand.Configure(command, options));
            app.Command("disable", command => DisableIdentityCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private static RemoveCommandOptions GetRemoveCommandOptions() =>
            new RemoveCommandOptions
            {
                Type = "identity",
                ArgumentName = "identity",
                ArgumentDescription = "The identity to remove",
                RemoveCommand = value => new RemoveCommand(async context => await context.IdentityResourcesClient.RemoveIdentityResourceAsync(value).ConfigureAwait(false))
            };

        private static ShowCommandOptions GetShowCommandOptions() =>
            new ShowCommandOptions
            {
                Type = "identitie",
                ArgumentName = "identity",
                ArgumentDescription = "The identity (you can end the identity with a wildcard to search)",
                DisplayCommand = (string value) => new ShowCommand.Display<IdentityResource>(async context => await context.IdentityResourcesClient.GetIdentityResourceAsync(value).ConfigureAwait(false)),
                ListCommand = (string startsWith, int skip, int take) =>
                    new ShowCommand.List<ResourceSummary>(
                        "identities",
                        async context => await context.IdentityResourcesClient.GetIdentityResourceSummariesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("name", identity => identity.Name),
                        ("display_name", identity => identity.DisplayName),
                        ("enabled", client => client.Enabled.ToString(CultureInfo.InvariantCulture)))
            };
    }
}