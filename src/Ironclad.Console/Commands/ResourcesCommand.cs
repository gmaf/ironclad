// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ResourcesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides resources related operations";
            app.HelpOption();

            // commands
            app.Command("api", command => ConfigureApiResources(command, options));
            app.Command("identity", command => ConfigureIdentityResources(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }

        private static void ConfigureApiResources(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Provides API resources related operations";
            app.HelpOption();

            // commands
            app.Command("list", command => ListApiResourcesCommand.Configure(command, options));
            app.Command("add", command => AddApiResourceCommand.Configure(command, options));
            app.Command("show", command => ShowApiResourceCommand.Configure(command, options));
            app.Command("remove", command => RemoveApiResourceCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }

        private static void ConfigureIdentityResources(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Provides API resources related operations";
            app.HelpOption();

            // commands
            app.Command("list", command => ListIdentityResourcesCommand.Configure(command, options));
            app.Command("add", command => AddIdentityResourceCommand.Configure(command, options));
            app.Command("show", command => ShowIdentityResourceCommand.Configure(command, options));
            app.Command("remove", command => RemoveIdentityResourceCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());
        }
    }
}