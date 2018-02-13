// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using Microsoft.Extensions.Configuration;
    using Npgsql;

    public sealed class IroncladFixture : IDisposable
    {
        private const string ConnectionString = "Host=localhost;Database=ironclad;Username=postgres;Password=postgres;";

        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process postgresProcess;
        private readonly Process ironcladProcess;

        public IroncladFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            this.Authority = config.GetValue<string>("authority");

            this.postgresProcess = this.StartPostgres();
            this.ironcladProcess = this.StartIronclad();
        }

        public string Authority { get; }

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

        private Process StartPostgres()
        {
            var process = Process.Start(
                new ProcessStartInfo("docker", $"run --name {DockerContainerId} -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ironclad -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            using (var connection = new NpgsqlConnection(ConnectionString))
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

        private Process StartIronclad()
        {
            var path = string.Format(
                CultureInfo.InvariantCulture,
                "..{0}..{0}..{0}..{0}..{0}Ironclad{0}bin{0}Debug{0}netcoreapp2.0{0}Ironclad.dll",
                Path.DirectorySeparatorChar);

            var process = Process.Start(
                new ProcessStartInfo("dotnet", $"{path} --connectionString {ConnectionString}")
                {
                    UseShellExecute = true,
                });

            using (var client = new HttpClient())
            {
                var attemp = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        using (var response = client.GetAsync(new Uri(this.Authority + "/.well-known/openid-configuration")).Result)
                        {
                        }

                        break;
                    }
                    catch (Exception)
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
    }
}

// run ..\..\..\..\..\Ironclad\Ironclad.csproj --connectionString Host=localhost;Database=ironclad;Username=postgres;Password=integration;