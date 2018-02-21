// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddRoleCommand : ICommand
    {
        private string role;

        private AddRoleCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Creates a new role on the authorization server.";

            // arguments
            var argumentRole = app.Argument("name", "The role name.", false);

            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRole.Value))
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    options.Command = new AddRoleCommand { role = argumentRole.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.RolesClient.AddRoleAsync(this.role).ConfigureAwait(false);
    }
}
