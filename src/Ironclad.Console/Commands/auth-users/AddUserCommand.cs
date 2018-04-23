// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class AddUserCommand : ICommand
    {
        private User user;

        private AddUserCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Creates a new user";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'users add -i' to enter interactive mode{Environment.NewLine}";

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);

            // options
#pragma warning disable SA1025
            var optionPassword =              app.Option("-p|--password <password>",             "The password",                                                       CommandOptionType.SingleValue);
            var optionEmail =                 app.Option("-e|--email <email>",                   "The email address for the user",                                     CommandOptionType.SingleValue);
            var optionPhoneNumber =           app.Option("-n|--phone <phone>",                   "The phone number for the user",                                      CommandOptionType.SingleValue);
            var optionRoles =                 app.Option("-r|--role <role>",                     "A role to assign the new user to (you can call this several times)", CommandOptionType.MultipleValue);
            var optionExternalLoginProvider = app.Option("-l|--login_provider <login_provider>", "The external login provider",                                        CommandOptionType.SingleValue);
            var optionInteractive =           app.Option("-i|--interactive",                     "Enters interactive mode",                                            CommandOptionType.NoValue);
#pragma warning restore SA1025

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value) && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new UserHelper();

                    var user = new User
                    {
                        Username = argumentUsername.Value,
                        Password = optionPassword.Value(),
                        Email = optionEmail.Value(),
                        PhoneNumber = optionPhoneNumber.Value(),
                        Roles = optionRoles.HasValue() ? optionRoles.Values.Distinct().ToHashSet() : null,
                        ExternalLoginProvider = optionExternalLoginProvider.Value()
                    };

                    reporter.Verbose("Prototype user (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(user));

                    if (!helper.IsValid(user) || optionInteractive.HasValue())
                    {
                        try
                        {
                            user = helper.GetValid(user);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated user (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(user));
                    }

                    options.Command = new AddUserCommand { user = user };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.UsersClient.AddUserAsync(this.user).ConfigureAwait(false);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private class UserHelper : IHelper<User>
        {
            public User GetPrototype(User user) => user;

            public bool IsValid(User user) => !string.IsNullOrEmpty(user.Username);

            public User GetValid(User user)
            {
                user.Username = Safe(Prompt.GetString("Username:", user.Username), "Cannot create a user without a username.");
                user.Password = user.Password ?? Prompt.GetPassword("Password:");
                user.Roles = Prompt.GetString("Assigned roles for the user (space separated) [optional]:", user.Roles == null ? null : string.Join(' ', user.Roles))
                    ?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // defaults
                user.Password = string.IsNullOrWhiteSpace(user.Password) ? null : user.Password;

                return user;
            }
        }
    }
}
