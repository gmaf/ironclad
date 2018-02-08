// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.


namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;

    public sealed class IroncladFixture : IDisposable
    {
        private static readonly string DockerContainerId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(12);

        private readonly Process postgresProcess;
        private readonly Process ironcladProcess;

        public IroncladFixture()
        {
            this.postgresProcess = Process.Start(
                new ProcessStartInfo("docker", $"run --name {DockerContainerId} -e POSTGRES_PASSWORD=password -e POSTGRES_DB=ironclad -p 5432:5432 postgres:10.1-alpine")
                {
                    UseShellExecute = true,
                });

            // TODO (Cameron): This should be configurable.
            Thread.Sleep(5000);

            var d = Path.DirectorySeparatorChar;
            this.ironcladProcess = Process.Start(
                new ProcessStartInfo("dotnet", $"..{d}..{d}..{d}..{d}..{d}Ironclad{d}bin{d}Debug{d}netcoreapp2.0{d}Ironclad.dll")
                {
                    UseShellExecute = true,
                });

            // TODO (Cameron): This should be configurable.
            Thread.Sleep(5000);
        }

        public void Dispose()
        {
            this.ironcladProcess.Kill();
            this.ironcladProcess.Dispose();

            this.postgresProcess.Kill();
            this.postgresProcess.Dispose();

            // NOTE (Cameron): Remove the docker container.
            Process.Start(new ProcessStartInfo("docker", $"rm {DockerContainerId} -f"))
                .WaitForExit(5000);
        }
    }
}