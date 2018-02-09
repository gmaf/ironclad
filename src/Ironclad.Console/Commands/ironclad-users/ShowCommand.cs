// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowCommand : ICommand
    {
        private string userId;

        private ShowCommand()
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

                    options.Command = new ShowCommand { userId = argumentUserId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = await context.Client.GetUserAsync(this.userId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(client, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}
