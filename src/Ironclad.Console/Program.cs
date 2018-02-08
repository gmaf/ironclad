// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using Ironclad.Console.Commands;
    using McMaster.Extensions.CommandLineUtils;

    internal class Program
    {
        private readonly IConsole console;

        public Program(IConsole console)
        {
            this.console = console;
        }

        public static Task<int> Main(string[] args)
        {
            Sdk.DebugHelper.HandleDebugSwitch(ref args);

            return new Program(PhysicalConsole.Singleton).TryRunAsync(args);
        }

        public async Task<int> TryRunAsync(string[] args)
        {
            CommandLineOptions options;
            try
            {
                options = CommandLineOptions.Parse(args, this.console);
            }
            catch (CommandParsingException ex)
            {
                await this.console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
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

            if (options.Command == null)
            {
                return 3;
            }

            // NOTE (Cameron): This is basically setting up CommandContext as a container for our commands.
            using (var client = new IroncladClient("http://localhost:5005"))
            {
                var context = new CommandContext(this.console, client);

                try
                {
                    await options.Command.ExecuteAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await this.console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                }

                return 0;
            }
        }
    }
}
