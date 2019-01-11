// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    internal interface IHelper<T>
    {
        T GetPrototype(T item);

        bool IsValid(T item);

        T GetValid(T item);
    }
}