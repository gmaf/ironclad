// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class ApiResourcesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Provides API resources related operations";
            app.HelpOption();

            // commands
            app.Command("add", command => AddApiResourceCommand.Configure(command, options));
            app.Command("show", command => ShowApiResourcesCommand.Configure(command, options));
            app.Command("remove", command => RemoveApiResourceCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }
    }
}