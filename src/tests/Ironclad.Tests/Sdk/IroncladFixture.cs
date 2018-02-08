// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public sealed class IroncladFixture : IDisposable
    {
        private readonly Process process;

        public IroncladFixture()
        {
            // TODO (Cameron): Configure new Postgres database (container?)
            this.process = Process.Start(
                new ProcessStartInfo("dotnet", @"..\..\..\..\..\Ironclad\bin\Debug\netcoreapp2.0\Ironclad.dll")
                {
                    UseShellExecute = true,
                });

            // NOTE (Cameron): We need to give time to the process to start.
            // TODO (Cameron): This should be configurable.
            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            this.process.Kill();
            this.process.Dispose();
        }
    }
}
