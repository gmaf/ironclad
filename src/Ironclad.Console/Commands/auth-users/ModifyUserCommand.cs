// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal class ModifyUserCommand : ICommand
    {
        private string username;
        private string email;
        private string phone;

        private ModifyUserCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Modifies a user";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);

            // options
            var optionsEmail = app.Option("-e|--email", "The email address for the user", CommandOptionType.SingleValue);
            var optionsPhone = app.Option("-p|--phone", "The phone number for the user", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value) || (string.IsNullOrEmpty(optionsEmail.Value()) && string.IsNullOrEmpty(optionsPhone.Value())))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ModifyUserCommand { username = argumentUsername.Value, email = optionsEmail.Value(), phone = optionsPhone.Value() };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new User
            {
                Id = this.username,
                Email = this.email,
                PhoneNumber = this.phone,
            };

            await context.UsersClient.ModifyUserAsync(user).ConfigureAwait(false);
        }
    }
}
