// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using Ironclad.Console.Sdk;
    using Microsoft.Extensions.CommandLineUtils;

    internal class ListCommand : ICommand
    {
        private int skip;
        private int take;

        private ListCommand()
        {
        }

        public static void Configure(CommandLineApplication command, CommandLineOptions options, IConsole console)
        {
            command.Description = "Lists the registered clients";
            command.HelpOption("-?|-h|--help");

            var skipArg = command.Argument("[skip]", "The number of clients to skip");
            var takeArg = command.Argument("[take]", "The number of clients to take");

            command.OnExecute(
                () =>
                {
                    var skip = 0;
                    if (!string.IsNullOrEmpty(skipArg.Value) && !int.TryParse(skipArg.Value, out skip))
                    {
                        throw new CommandParsingException(command, $"Unable to parse [skip] value of '{skipArg.Value}'");
                    }

                    var take = 20;
                    if (!string.IsNullOrEmpty(takeArg.Value) && !int.TryParse(takeArg.Value, out take))
                    {
                        throw new CommandParsingException(command, $"Unable to parse [take] value of '{takeArg.Value}'");
                    }

                    options.Command = new ListCommand { skip = skip, take = take };
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var clients = await context.Client.GetClientSummariesAsync(this.skip, this.take).ConfigureAwait(false);
            foreach (var client in clients)
            {
                await context.Console.Out.WriteLineAsync($"{client.Id}: ({client.Name})").ConfigureAwait(false);
            }

            await context.Console.Out.WriteLineAsync($"Showing from {clients.Start + 1} to {clients.Start + clients.Size} of {clients.TotalSize} in total.")
                .ConfigureAwait(false);
        }
    }
}
