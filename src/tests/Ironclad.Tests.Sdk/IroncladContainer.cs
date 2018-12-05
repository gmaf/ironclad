// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class IroncladContainer : Container
    {
        private readonly IroncladProbe probe;

        public IroncladContainer(string authority, string connectionString)
        {
            if (authority == null)
            {
                throw new ArgumentNullException(nameof(authority));
            }

            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.Configuration = new ContainerConfiguration
            {
                Image = "ironclad",
                Tag = "dev",
                IsContainerReusable = true,
                ContainerName = "ironclad-integration",
                ContainerPortBindings = new[]
                {
                    new ContainerConfiguration.PortBinding
                    {
                        GuestTcpPort = 80, HostTcpPort = 5005
                    }
                },
                ContainerEnvironmentVariables = new[]
                {
                    $"IRONCLAD_CONNECTIONSTRING={connectionString}",

                    // NOTE (Cameron): Not secret. Only functional for development eg. localhost.
                    $"GOOGLE_CLIENT_ID=835517018777-4hnr0i9s8750kb10uaejdokel68bhtbb.apps.googleusercontent.com",
                    $"GOOGLE_SECRET=LCPH4fgebc-i4JR99GmoYU-X",
                },
            };

            this.probe = new IroncladProbe(authority, 0, 20);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);
            await this.probe.WaitUntilAvailable(true, default).ConfigureAwait(false);
        }
    }
}