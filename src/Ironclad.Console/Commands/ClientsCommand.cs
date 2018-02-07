// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using Ironclad.Console.Sdk;
    using Microsoft.Extensions.CommandLineUtils;

    internal class ClientsCommand : ICommand
    {
        private ClientsCommand()
        {
        }

        public static void Configure(CommandLineApplication command, CommandLineOptions options, IConsole console)
        {
            command.Description = "Provides client related operations";
            command.HelpOption();

            command.Command("list", cmd => ListCommand.Configure(cmd, options, console));

            command.OnExecute(
                () =>
                {
                    command.ShowHelp();

                    options.Command = new ClientsCommand();
                });
        }

        public Task ExecuteAsync(CommandContext context) => Task.CompletedTask;
    }
}
