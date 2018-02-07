// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Sdk
{
    using System;
    using System.IO;

    public interface IConsole
    {
        event ConsoleCancelEventHandler CancelKeyPress;

        TextWriter Out { get; }

#pragma warning disable CA1716
        TextWriter Error { get; }

        TextReader In { get; }
#pragma warning restore CA1716

        bool IsInputRedirected { get; }

        bool IsOutputRedirected { get; }

        bool IsErrorRedirected { get; }

        ConsoleColor ForegroundColor { get; set; }

        void ResetColor();
    }
}