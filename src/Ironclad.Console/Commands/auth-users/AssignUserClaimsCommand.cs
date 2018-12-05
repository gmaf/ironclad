// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal class AssignUserClaimsCommand : ICommand
    {
        private string username;
        private Dictionary<string, object> claims;

        private AssignUserClaimsCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Assigns claims to a user";
            app.HelpOption();

            // arguments
            var argumentUsername = app.Argument("username", "The username", false);
            var argumentClaims = app.Argument("claims", "One or more claims, formatted as type=value, to assign to the user (you can specify multiple claims)", true);

            var optionRemove = app.Option("-r|--remove", "Removes all the claims", CommandOptionType.NoValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (
                        string.IsNullOrEmpty(argumentUsername.Value) ||
                        (!optionRemove.HasValue() && (!argumentClaims.Values.Any() || argumentClaims.Values.Any(value => !value.Contains("=", StringComparison.OrdinalIgnoreCase)))))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new AssignUserClaimsCommand
                    {
                        username = argumentUsername.Value,
                        claims = argumentClaims.Values.ToDictionary(key => key.Split('=').First(), value => (object)value.Split('=').Last()),
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var user = new User
            {
                Username = this.username,
                Roles = null,
                Claims = this.claims,
            };

            await context.UsersClient.ModifyUserAsync(user).ConfigureAwait(false);
        }
    }
}