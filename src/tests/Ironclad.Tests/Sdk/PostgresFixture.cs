// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Npgsql;

    public sealed class PostgresFixture : IDisposable
    {
        internal const string ConnectionString = "Host=localhost;Database=ironclad;Username=postgres;Password=postgres;";

        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process process;

        public PostgresFixture()
        {
            this.process = this.StartPostgresProcess();
        }

        public void Dispose()
        {
            try
            {
                this.process.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.process.Dispose();

            // NOTE (Cameron): Remove the docker container.
            Process.Start(new ProcessStartInfo("docker", $"stop {DockerContainerId}")).WaitForExit(10000);
        }

        private Process StartPostgresProcess()
        {
            var process = Process.Start(
                new ProcessStartInfo("docker", $"run --rm --name {DockerContainerId} -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ironclad -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            // NOTE (Cameron): Trying to find a sensible value here so as to not throw during a debug session.
            Thread.Sleep(4000);

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var attempt = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    try
                    {
                        connection.Open();
                        break;
                    }
                    catch (Exception ex) when (ex is NpgsqlException || ex is SocketException || ex is EndOfStreamException)
                    {
                        if (++attempt >= 20)
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