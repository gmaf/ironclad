// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ShowApiResourceCommand : ICommand
    {
        private string resourceName;

        private ShowApiResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = $"Show the specified API resource";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("resourceName", "The resource name to show", false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        app.ShowHelp();
                        return;
                    }

                    options.Command = new ShowApiResourceCommand { resourceName = argumentClientId.Value };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var resource = await context.ApiResourcesClient.GetApiResourceAsync(this.resourceName).ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync(JsonConvert.SerializeObject(resource)).ConfigureAwait(false);
        }
    }
}