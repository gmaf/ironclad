// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Sdk
{
    using System;
    using System.IO;

    public class PhysicalConsole : IConsole
    {
        private PhysicalConsole()
        {
            Console.CancelKeyPress += (o, e) => this.CancelKeyPress?.Invoke(o, e);
        }

        public event ConsoleCancelEventHandler CancelKeyPress;

        public static IConsole Singleton { get; } = new PhysicalConsole();

        public TextWriter Error => Console.Error;

        public TextReader In => Console.In;

        public TextWriter Out => Console.Out;

        public bool IsInputRedirected => Console.IsInputRedirected;

        public bool IsOutputRedirected => Console.IsOutputRedirected;

        public bool IsErrorRedirected => Console.IsErrorRedirected;

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void ResetColor() => Console.ResetColor();
    }
}