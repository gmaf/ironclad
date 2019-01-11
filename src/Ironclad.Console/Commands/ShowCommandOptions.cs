// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;

    public class ShowCommandOptions
    {
        public string Type { get; set; }

        public string ArgumentName { get; set; }

        public string ArgumentDescription { get; set; }

        public Func<string, ICommand> DisplayCommand { get; set; }

        public Func<string, int, int, ICommand> ListCommand { get; set; }
    }
}