// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;

    public interface ICommand
    {
        Task ExecuteAsync(CommandContext context);
    }
}
