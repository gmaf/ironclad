// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal class AssignUserRolesCommand : ICommand
    {
        private string username;
        private List<string> roles;

        private AssignUserRolesCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Assigns roles to a user";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);
            var argumentRoles = app.Argument("roles", "One or more roles to assign to the user (you can specify multiple roles)", true);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value) || !argumentRoles.Values.Any())
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AssignUserRolesCommand { username = argumentUsername.Value, roles = argumentRoles.Values };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new User
            {
                Username = this.username,
                Roles = this.roles,
                Claims = null,
            };

            await context.UsersClient.ModifyUserAsync(user).ConfigureAwait(false);
        }
    }
}
