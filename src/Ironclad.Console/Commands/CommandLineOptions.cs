// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using Ironclad.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;

    public class CommandLineOptions
    {
        public bool IsHelp { get; private set; }

        public ICommand Command { get; set; }

        public static CommandLineOptions Parse(string[] args, IConsole console)
        {
            // NOTE (Cameron): We need to pass the options through each of the commands before we can evaluate the result of the parsing.
            var options = new CommandLineOptions();

            var app = new CommandLineApplication(console);

            app.HelpOption();

            // commands
            app.Command("login", command => LoginCommand.Configure(command, options, console));
            app.Command("clients", command => ClientsCommand.Configure(command, options, console));
            app.Command("apis", command => ApiResourcesCommand.Configure(command, options, console));
            app.Command("users", command => UsersCommand.Configure(command, options));
            app.Command("roles", command => RolesCommand.Configure(command, options));

            // action (for this command)
            app.OnExecute(() => app.ShowVersionAndHelp());

            // NOTE (Cameron): The result of a successful execute will assign a command to the options which gets invoked elsewhere.
            if (app.Execute(args) != 0)
            {
                // when command line parsing error in subcommand
                return null;
            }

            options.IsHelp = app.IsShowingInformation;

            return options;
        }
    }
}
