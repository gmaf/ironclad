// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

// TODO (Cameron): Remove suppression (below) when StyleCop supports C# 7.0 tuples.
#pragma warning disable SA1008

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;

    internal class ListCommand<T> : ICommand
    {
        private readonly string type;
        private readonly Func<CommandContext, Task<ResourceSet<T>>> query;
        private readonly List<string> columnNames = new List<string>();
        private readonly List<Func<T, string>> columns = new List<Func<T, string>>();

        public ListCommand(string type, Func<CommandContext, Task<ResourceSet<T>>> query, (string, Func<T, string>) column1, params (string, Func<T, string>)[] columns)
        {
            this.type = type;
            this.query = query;
            this.columnNames.Add(column1.Item1);
            this.columnNames.AddRange(columns.Select(tuple => tuple.Item1));
            this.columns.Add(column1.Item2);
            this.columns.AddRange(columns.Select(tuple => tuple.Item2));
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var results = await this.query(context).ConfigureAwait(false);
            if (!results.Any())
            {
                context.Reporter.QueryReturnedNoResults(results, this.type);
                return;
            }

            // NOTE (Cameron): Madness.
            var format = " ";
            foreach (var column in this.columns.Take(this.columns.Count - 1))
            {
                format += string.Format(
                    CultureInfo.InvariantCulture,
                    $"{{{{{this.columns.IndexOf(column)}, -{{0}}}}}}",
                    results.Max(result => column(result)?.Length ?? 0) + 2);
            }

            format += $"{{{this.columns.Count - 1}}}";

            await context.Console.Out.WriteLineAsync().ConfigureAwait(false);

            context.Console.ForegroundColor = ConsoleColor.White;
            context.Console.Out.WriteLine(format, this.columnNames.ToArray());
            context.Console.ResetColor();

            foreach (var user in results)
            {
                context.Console.Out.WriteLine(format, this.columns.Select(column => column(user)).ToArray());
            }

            await context.Console.Out.WriteLineAsync().ConfigureAwait(false);
            await context.Console.Out.WriteLineAsync($"Showing from {results.Start + 1} to {results.Start + results.Size} of {results.TotalSize} {this.type} in total.")
                .ConfigureAwait(false);
        }
    }
}
