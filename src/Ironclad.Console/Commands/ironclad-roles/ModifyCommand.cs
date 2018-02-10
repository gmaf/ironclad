// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Roles
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ModifyCommand : ICommand
    {
        private string roleId;
        private string name;

        private ModifyCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Registers the specified role";
            app.HelpOption();

            // arguments
            var argumentRoleId = app.Argument("id", "The role ID", false);
            var argumentName = app.Argument("name", "The role name", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleId.Value) || string.IsNullOrEmpty(argumentName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ModifyCommand { roleId = argumentRoleId.Value, name = argumentName.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var role = new Ironclad.Client.Role
            {
                Id = this.roleId,
                Name = this.name,
            };

            await context.Client.ModifyRoleAsync(role).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
