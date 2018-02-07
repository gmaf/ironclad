// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using console = System.Console;

    internal class ProgramOld
    {
        public static async Task Xyz()
        {
            console.WriteLine("Ironclad");

            var command = string.Empty;
            while (!string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await Handle(command).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    console.WriteLine(ex.Message);
                }

                console.Write("> ");
                command = console.ReadLine().Trim();
            }

            console.WriteLine("Goodbye!");
        }

        private static async Task Handle(string command)
        {
            var commandParts = command.Split(new[] { ' ' });
            switch (commandParts.First().ToUpperInvariant())
            {
                case "?":
                case "HELP":
                    console.WriteLine(@"Commands:
    list [skip] [take] - lists all the registered clients
    register [reg]     - registers a car with the [reg]
    drive [reg] [dist] - drives the car with the [reg] the specified [dist]
    scrap [reg]        - scraps the car with the [reg]");
                    break;

                case "LIST":
                    var skip = 0;
                    if (commandParts.Length > 1 && !int.TryParse(commandParts[1], out skip))
                    {
                        console.WriteLine("[skip] must be a number.");
                        break;
                    }

                    var take = 20;
                    if (commandParts.Length > 2 && !int.TryParse(commandParts[2], out take))
                    {
                        console.WriteLine("[take] must be a number.");
                        break;
                    }

                    // list cars
                    using (var ironcladClient = new IroncladClient("http://localhost:5005"))
                    {
                        var clients = await ironcladClient.GetClientSummariesAsync(skip, take).ConfigureAwait(false);
                        foreach (var client in clients)
                        {
                            console.WriteLine($"{client.Id}: ({client.Name})");
                        }

                        console.WriteLine($"Showing from {clients.Start + 1} to {clients.Start + clients.Size} of {clients.TotalSize} in total.");
                    }

                    break;

                case "":
                    break;

                default:
                    Console.WriteLine("Eh?");
                    break;
            }
        }
    }
}
