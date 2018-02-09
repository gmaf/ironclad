// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class UnregisterCommand : ICommand
    {
        private string userId;

        private UnregisterCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Unregisters the specified client";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("id", "The user ID", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new UnregisterCommand { userId = argumentUserId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Client.UnregisterUserAsync(this.userId).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}