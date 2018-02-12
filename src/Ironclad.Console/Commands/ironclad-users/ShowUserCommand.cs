// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowUserCommand : ICommand
    {
        private string userId;

        private ShowUserCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Shows the specified user";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("username", "The username of the user to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowUserCommand { userId = argumentUserId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = await context.UsersClient.GetUserAsync(this.userId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(user)).ConfigureAwait(false);
        }
    }
}
