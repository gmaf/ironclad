// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RemoveRoleCommand : ICommand
    {
        private string role;

        private RemoveRoleCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Removes the specified role";
            app.HelpOption();

            // arguments
            var argumentRoleId = app.Argument("name", "The role name", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RemoveRoleCommand { role = argumentRoleId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.RolesClient.RemoveRoleAsync(this.role).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}