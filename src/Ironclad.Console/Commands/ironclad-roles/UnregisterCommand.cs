// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Roles
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class UnregisterCommand : ICommand
    {
        private string roleId;

        private UnregisterCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Unregisters the specified client";
            app.HelpOption();

            // arguments
            var argumentRoleId = app.Argument("id", "The role ID", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new UnregisterCommand { roleId = argumentRoleId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Client.UnregisterRoleAsync(this.roleId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}