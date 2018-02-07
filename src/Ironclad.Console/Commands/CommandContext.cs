// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using Ironclad.Client;
    using Ironclad.Console.Sdk;

    public class CommandContext
    {
        public CommandContext(IConsole console, IIroncladClient client)
        {
            this.Console = console;
            this.Client = client;
        }

        public IConsole Console { get; }

        public IIroncladClient Client { get; }
    }
}
