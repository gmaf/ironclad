using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ironclad.Client;
using McMaster.Extensions.CommandLineUtils;

namespace Ironclad.Console.Commands
{
    internal class AssignUserClaimsCommand : ICommand
    {
        private string username;
        private List<UserClaim> claims;

        private AssignUserClaimsCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Assigns roles to a user";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);
            var argumentClaims = app.Argument("claims", "One or more claims, formatted as type=value, to assign to the user (you can specify multiple)", true);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentUsername.Value) || !argumentClaims.Values.Any())
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AssignUserClaimsCommand
                    {
                        username = argumentUsername.Value,
                        claims = argumentClaims.Values
                            .ConvertAll(value =>
                            {
                                var parts = value.Split('=');
                                if (parts.Length == 2)
                                {
                                    return new UserClaim
                                    {
                                        Type = parts[0],
                                        Value = parts[1]
                                    };
                                }

                                return null;
                            })
                            .Where(claim => claim != null)
                            .ToList()
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new User
            {
                Username = this.username,
                UserClaims = this.claims
            };

            await context.UsersClient.ModifyUserAsync(user).ConfigureAwait(false);
        }
    }
}