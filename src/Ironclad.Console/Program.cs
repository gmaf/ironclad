// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using System.Threading.Tasks;
    using Ironclad.Client;
    using Ironclad.Console.Commands;
    using Ironclad.Console.Sdk;
    using Microsoft.Extensions.CommandLineUtils;

    internal class Program
    {
        private readonly IConsole console;

        public Program(IConsole console)
        {
            this.console = console;
        }

        public static Task<int> Main(string[] args)
        {
            DebugHelper.HandleDebugSwitch(ref args);
            return new Program(PhysicalConsole.Singleton).TryRunAsync(args);
        }

        public async Task<int> TryRunAsync(string[] args)
        {
            CommandLineOptions options;
            try
            {
                options = CommandLineOptions.Parse(args, this.console);
            }
            catch (CommandParsingException)
            {
                return 1;
            }

            if (options == null)
            {
                return 1;
            }

            if (options.IsHelp)
            {
                return 2;
            }

            using (var client = new IroncladClient("http://localhost:5005"))
            {
                var context = new CommandContext(this.console, client);
                await options.Command.ExecuteAsync(context).ConfigureAwait(false);
                return 0;
            }
        }
    }
}
