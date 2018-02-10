// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Roles
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowCommand : ICommand
    {
        private string roleId;

        private ShowCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the specified role";
            app.HelpOption();

            // arguments
            var argumentRoleId = app.Argument("id", "The role ID to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentRoleId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowCommand { roleId = argumentRoleId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var role = await context.Client.GetRoleAsync(this.roleId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(role, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}
