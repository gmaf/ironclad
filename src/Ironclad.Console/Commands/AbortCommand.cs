// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;

    public class AbortCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.Console.Out.WriteLineAsync("Aborted!").ConfigureAwait(false);
        }
    }
}