// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal static class ReporterExtensions
    {
        public static void QueryReturnedNoResults<T>(this IReporter reporter, ResourceSet<T> resourceSet, string type)
        {
            if (resourceSet.Start > resourceSet.TotalSize)
            {
                reporter.Warn($"Query returned no {type}. Consider setting the specified number of {type} to skip ({resourceSet.Start:N0}) to less than the total number of {type} in the system ({resourceSet.TotalSize:N0}).");
            }
            else
            {
                reporter.Output($"Query returned no {type}.");
            }
        }
    }
}
