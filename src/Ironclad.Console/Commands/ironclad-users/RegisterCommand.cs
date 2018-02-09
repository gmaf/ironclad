// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RegisterCommand : ICommand
    {
        private string username;
        private string password;
        private string email;
        private string phone;

        private RegisterCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Registers the specified user";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);
            var argumentPassword = app.Argument("password", "The password", false);

            // options
            var optionsEmail = app.Option("-e|--email", "The email address for the user", CommandOptionType.SingleValue);
            var optionsPhone = app.Option("-p|--phone", "The phone number for the user", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value) || string.IsNullOrEmpty(argumentPassword.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new RegisterCommand { username = argumentUsername.Value, password = argumentPassword.Value, email = optionsEmail.Value(), phone = optionsPhone.Value() };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new Ironclad.Client.User
            {
                Username = this.username,
                Password = this.password,
                Email = this.email,
                PhoneNumber = this.phone,
            };

            await context.Client.RegisterUserAsync(user).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
