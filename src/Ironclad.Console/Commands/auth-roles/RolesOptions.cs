// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    // NOTE (Cameron): This command is informational only and cannot be executed (only 'show help' works) so inheriting ICommand is unnecessary.
    internal static class RolesOptions
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Provides role related operations";
            app.HelpOption();

            // commands
            app.Command("show", command => CommonShowCommand.Configure(command, options, new RolesShowTraits()));
            app.Command("add", command => AddRoleCommand.Configure(command, options));
            app.Command("remove", command => RemoveRoleCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());
        }

        private class RolesShowTraits : IShowTraits
        {
            public string Name => "role";

            public string ArgumentName => "role";

            public string ArgumentDescription => "The role. You can end the role with a wildcard.";

            public ICommand GetShowCommand(string value) => new ExistsCommand { RoleName = value };

            public ICommand GetListCommand(string startsWith, int skip, int take) => new ListCommand<string>(
                "roles",
                async context => await context.RolesClient.GetRolesAsync(startsWith, skip, take).ConfigureAwait(false),
                ("role", role => role));

            private class ExistsCommand : ICommand
            {
                public string RoleName { get; set; }

                public async Task ExecuteAsync(CommandContext context)
                {
                    var exists = await context.RolesClient.RoleExistsAsync(this.RoleName).ConfigureAwait(false);
                    context.Reporter.Output(exists ? this.RoleName : $"Role '{this.RoleName}' not found");
                }
            }
        }
    }
}