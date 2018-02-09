// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class AssignRolesCommand : ICommand
    {
        private string userId;
        private List<string> roles;

        private AssignRolesCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Assign the roles for the specified user";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("id", "The user ID", false);
            var argumentRoles = app.Argument("roles", "One or more roles to assign to the user", true);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value) || !argumentRoles.Values.Any())
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AssignRolesCommand { userId = argumentUserId.Value, roles = argumentRoles.Values };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Client.AssignRolesToUserAsync(this.userId, this.roles).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
