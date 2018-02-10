// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Roles
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RegisterCommand : ICommand
    {
        private string roleName;

        private RegisterCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Registers the specified role";
            app.HelpOption();

            // arguments
            var argumentRoleName = app.Argument("name", "The Role Name", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RegisterCommand { roleName = argumentRoleName.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var role = new Ironclad.Client.Role
            {
                Name = this.roleName,
            };

            await context.Client.RegisterRoleAsync(role).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
