using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Ironclad.Console.Commands
{
    internal class ModifyIdentityCommand : ICommand
    {
        private string name;
        private ICollection<string> userClaims;

        private ModifyIdentityCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Enable an identity resource";
            app.HelpOption();

            // arguments
            var argumentName = app.Argument("name", "The name of the identity resource", false);
            var argumentUserClaims = app.Argument("user_claims", "The user claim types associated with the identity resource (you can specify multiple)", true);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentName.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ModifyIdentityCommand
                    {
                        name = argumentName.Value,
                        userClaims = argumentUserClaims.Values
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var identityResource = new Client.IdentityResource
            {
                Name = this.name,
                UserClaims = this.userClaims
            };

            await context.IdentityResourcesClient.ModifyIdentityResourceAsync(identityResource).ConfigureAwait(false);
        }
    }
}