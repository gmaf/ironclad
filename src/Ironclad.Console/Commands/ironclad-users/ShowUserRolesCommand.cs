// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowUserRolesCommand : ICommand
    {
        private string userId;

        private ShowUserRolesCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the specified user";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("userId", "The user ID to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowUserRolesCommand { userId = argumentUserId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var userRoles = await context.Client.GetUserRolesAsync(this.userId).ConfigureAwait(false);

            if (userRoles.Any())
            {
                await context.Console.Out.WriteLineAsync("Following roles are assigned to this user").ConfigureAwait(false);
            }
            else
            {
                await context.Console.Out.WriteLineAsync("No role is assigned to this user").ConfigureAwait(false);
            }

            foreach (var user in userRoles)
            {
                await context.Console.Out.WriteLineAsync(user.Name).ConfigureAwait(false);
            }
        }
    }
}
