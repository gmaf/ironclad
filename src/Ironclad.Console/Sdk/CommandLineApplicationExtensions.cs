// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Sdk
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.CommandLineUtils;

    internal static class CommandLineApplicationExtensions
    {
        public static CommandOption HelpOption(this CommandLineApplication app) => app.HelpOption("-?|-h|--help");

        public static void OnExecute(this CommandLineApplication app, Action action) =>
            app.OnExecute(
                () =>
                {
                    action();
                    return 0;
                });
    }
}