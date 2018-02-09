// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Users
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class ChangePasswordCommand : ICommand
    {
        private string userId;
        private string currentPassword;
        private string newPassword;

        private ChangePasswordCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Assign the roles for the specified user";
            app.HelpOption();

            // arguments
            var argumentUserId = app.Argument("id", "The user ID", false);
            var argumentCurrentPassword = app.Argument("currentPassword", "Current Password", false);
            var argumentNewPassword = app.Argument("newPassword", "New Password", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUserId.Value)
                        || string.IsNullOrEmpty(argumentCurrentPassword.Value)
                        || string.IsNullOrEmpty(argumentNewPassword.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ChangePasswordCommand
                        {
                            userId = argumentUserId.Value,
                            currentPassword = argumentCurrentPassword.Value,
                            newPassword = argumentNewPassword.Value
                        };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Client.ChangePasswordAsync(this.userId, this.currentPassword, this.newPassword).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync("Done!").ConfigureAwait(false);
        }
    }
}
