// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using Microsoft.Extensions.Configuration;
    using Npgsql;

    public sealed class IroncladFixture : IDisposable
    {
        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process postgresProcess;
        private readonly Process ironcladProcess;

        public IroncladFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();
            var authority = config.GetValue<string>("authority");

            this.postgresProcess = StartPostgres();
            this.ironcladProcess = StartIronclad();
        }

        public void Dispose()
        {
            try
            {
                this.ironcladProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.ironcladProcess.Dispose();

            try
            {
                this.postgresProcess.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.postgresProcess.Dispose();

            // NOTE (Cameron): Remove the docker container.
            Process.Start(new ProcessStartInfo("docker", $"rm {DockerContainerId} -f"))
                .WaitForExit(5000);
        }

        private static Process StartPostgres()
        {
            var process = Process.Start(
                new ProcessStartInfo("docker", $"run --name {DockerContainerId} -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ironclad -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            using (var connection = new NpgsqlConnection("Host=localhost;Database=ironclad;Username=postgres;Password=postgres;"))
            {
                var attemp = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        connection.Open();
                        break;
                    }
                    catch (NpgsqlException)
                    {
                        if (++attemp >= 20)
                        {
                            throw;
                        }
                    }
                }
            }

            return process;
        }

        private static Process StartIronclad()
        {
            var d = Path.DirectorySeparatorChar;

             var process = Process.Start(
                new ProcessStartInfo("dotnet", $"..{d}..{d}..{d}..{d}..{d}Ironclad{d}bin{d}Debug{d}netcoreapp2.0{d}Ironclad.dll --connectionString Host=localhost;Database=ironclad;Username=postgres;Password=postgres;")
                {
                    UseShellExecute = true,
                });

            // TODO (Cameron): This should be configurable.
            Thread.Sleep(7000);

            return process;
        }
    }
}

// run ..\..\..\..\..\Ironclad\Ironclad.csproj --connectionString Host=localhost;Database=ironclad;Username=postgres;Password=integration;