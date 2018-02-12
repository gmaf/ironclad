// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddRoleCommand : ICommand
    {
        private string roleName;

        private AddRoleCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Adds the specified role";
            app.HelpOption();

            // arguments
            var argumentRoleName = app.Argument("name", "The role name", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AddRoleCommand { roleName = argumentRoleName.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.RolesClient.AddRoleAsync(this.roleName).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
