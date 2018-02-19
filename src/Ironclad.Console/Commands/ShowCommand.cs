// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal class ShowCommand<T> : ICommand
    {
        private readonly Func<CommandContext, Task<T>> query;

        public ShowCommand(Func<CommandContext, Task<T>> query)
        {
            this.query = query;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var result = await this.query(context).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            context.Reporter.Output(json);
        }
    }
}
