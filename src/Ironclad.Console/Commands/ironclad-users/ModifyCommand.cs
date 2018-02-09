// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ModifyCommand : ICommand
    {
        private string userId;
        private string password;
        private string email;
        private string phone;

        private ModifyCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Registers the specified user";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("userId", "The user ID", false);

            // options
            var optionsEmail = app.Option("-e|--email", "The email address for the user", CommandOptionType.SingleValue);
            var optionsPhone = app.Option("-p|--phone", "The phone number for the user", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value) || (string.IsNullOrEmpty(optionsEmail.Value()) && string.IsNullOrEmpty(optionsPhone.Value())))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ModifyCommand { userId = argumentUserId.Value, email = optionsEmail.Value(), phone = optionsPhone.Value() };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new Ironclad.Client.User
            {
                Id = this.userId,
                Email = this.email,
                PhoneNumber = this.phone,
            };

            await context.Client.ModifyUserAsync(user).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
