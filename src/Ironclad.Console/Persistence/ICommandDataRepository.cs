// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Persistence
{
    public interface ICommandDataRepository
    {
        CommandData GetCommandData();

        void SetCommandData(CommandData commandData);
    }
}
