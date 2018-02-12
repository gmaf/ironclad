// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowIdentityResourceCommand : ICommand
    {
        private string resourceName;

        private ShowIdentityResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Show the specified identity resource";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("name", "The name of the resource to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowIdentityResourceCommand { resourceName = argumentClientId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var resource = await context.IdentityResourcesClient.GetIdentityResourceAsync(this.resourceName).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(resource)).ConfigureAwait(false);
        }
    }
}