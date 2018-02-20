// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    public interface IShowTraits
    {
        string Name { get; }

        string ArgumentName { get; }

        string ArgumentDescription { get; }

        ICommand GetShowCommand(string value);

        ICommand GetListCommand(string startsWith, int skip, int take);
    }
}