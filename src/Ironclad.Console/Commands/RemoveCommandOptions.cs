// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Threading.Tasks;

    public class RemoveCommandOptions
    {
        public string Type { get; set; }

        public string ArgumentName { get; set; }

        public string ArgumentDescription { get; set; }

        public Func<string, ICommand> RemoveCommand { get; set; }
    }
}