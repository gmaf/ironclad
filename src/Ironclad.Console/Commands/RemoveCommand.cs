// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class RemoveCommand : ICommand
    {
        private Func<CommandContext, Task> command;

        public RemoveCommand(Func<CommandContext, Task> command)
        {
            this.command = command;
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions commandLineOptions, RemoveCommandOptions removeCommandOptions)
        {
            // description
            app.Description = $"Removes the specified {removeCommandOptions.Type}.";
            app.HelpOption();

            // arguments
            var argument = app.Argument(removeCommandOptions.ArgumentName, removeCommandOptions.ArgumentDescription, false);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argument.Value))
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    commandLineOptions.Command = removeCommandOptions.RemoveCommand(argument.Value);
                });
        }

        public Task ExecuteAsync(CommandContext context) => this.command(context);
    }
}