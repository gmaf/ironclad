// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Reflection;
    using McMaster.Extensions.CommandLineUtils;

    public class CommandLineOptions
    {
        public bool IsHelp { get; private set; }

        public bool IsVerbose { get; private set; }

        public ICommand Command { get; set; }

        public static CommandLineOptions Parse(string[] args, IConsole console)
        {
            // NOTE (Cameron): We need to pass the options through each of the commands before we can evaluate the result of the parsing.
            var options = new CommandLineOptions();

            var app = new CommandLineApplication()
            {
                Out = console.Out,
                Error = console.Error,
                Name = "ironclad",
                FullName = "Ironclad Management Client",
                Description = "Manages Ironclad setup"
            };

            app.HelpOption();

            // options
            var optionVerbose = app.Option("-v|--verbose", "Show verbose output", CommandOptionType.NoValue, inherited: true);
            var optionVersion = app.VersionOption("--version", GetInformationalVersion(typeof(Program).Assembly));

            // commands
            app.Command("clients", command => ClientsCommand.Configure(command, options, console));
            app.Command("resources", command => ResourcesCommand.Configure(command, options));
            app.Command("users", command => UsersCommand.Configure(command, options, console));

            // action (for this command)
            app.OnExecute(() => app.ShowHelp());

            // NOTE (Cameron): The result of a successful execute will assign a command to the options which gets invoked elsewhere.
            if (app.Execute(args) != 0)
            {
                // when command line parsing error in subcommand
                return null;
            }

            options.IsHelp = app.IsShowingInformation;
            options.IsVerbose = optionVerbose.HasValue();

            return options;
        }

        private static string GetInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var versionAttribute = attribute == null
                ? assembly.GetName().Version.ToString()
                : attribute.InformationalVersion;

            return versionAttribute;
        }
    }
}
