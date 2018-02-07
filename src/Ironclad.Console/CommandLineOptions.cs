// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using Ironclad.Console.Commands;
    using Ironclad.Console.Sdk;
    using Microsoft.Extensions.CommandLineUtils;

    public class CommandLineOptions
    {
        public bool IsHelp { get; private set; }

        public bool IsVerbose { get; private set; }

        public ICommand Command { get; set; }

        public static CommandLineOptions Parse(string[] args, IConsole console)
        {
            var app = new CommandLineApplication
            {
                Out = console.Out,
                Error = console.Error,
                Name = "ironclad",
                FullName = "Ironclad Management Client",
                Description = "Manages Ironclad setup"
            };

            app.HelpOption();

            var optionVerbose = app.VerboseOption();

            app.VersionOptionFromAssemblyAttributes(typeof(Program).Assembly);

            var options = new CommandLineOptions();

            ////app.Command("set", c => SetCommand.Configure(c, options, console));
            ////app.Command("remove", c => RemoveCommand.Configure(c, options));
            app.Command("clients", command => ClientsCommand.Configure(command, options, console));
            ////app.Command("clear", c => ClearCommand.Configure(c, options));

            // Show help information if no subcommand/option was specified.
            app.OnExecute(() => app.ShowHelp());

            if (app.Execute(args) != 0)
            {
                // when command line parsing error in subcommand
                return null;
            }

            options.IsHelp = app.IsShowingInformation;
            options.IsVerbose = optionVerbose.HasValue();

            return options;
        }
    }
}
