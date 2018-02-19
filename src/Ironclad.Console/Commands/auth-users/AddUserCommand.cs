// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddUserCommand : ICommand
    {
        private User user;

        private AddUserCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Creates a new user.";

            // arguments
            var argumentUsername = app.Argument("username", "The username.", false);
            var argumentPassword = app.Argument("password", "The password.", false);

            // options
            var optionsEmail = app.Option("-e|--email <email>", "The email address for the user.", CommandOptionType.SingleValue);
            var optionsPhone = app.Option("-p|--phone <phone>", "The phone number for the user.", CommandOptionType.SingleValue);
            var optionsRoles = app.Option("-r|--role <role>", "A role to assign the new user to. You can call this several times.", CommandOptionType.MultipleValue);
            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value))
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var user = new User
                    {
                        Username = argumentUsername.Value,
                        Password = argumentPassword.Value,
                        Email = optionsEmail.Value(),
                        PhoneNumber = optionsPhone.Value(),
                        Roles = optionsRoles.Values,
                    };

                    options.Command = new AddUserCommand { user = user };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.UsersClient.AddUserAsync(this.user).ConfigureAwait(false);
    }
}
