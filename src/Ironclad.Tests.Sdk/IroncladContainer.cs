// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    internal class IroncladContainer : Container
    {
        private readonly IroncladProbe probe;

        public IroncladContainer(string authority, int port, string connectionString, string dockerRegistry, NetworkCredential dockerCredentials, string dockerTag)
        {
            authority = authority ?? throw new ArgumentNullException(nameof(authority));
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            this.Configuration = new ContainerConfiguration
            {
                Registry = dockerRegistry,
                RegistryCredentials = dockerCredentials,
                Image = "ironclad",
                Tag = dockerTag,
                ContainerName = "ironclad-integration",
                ContainerPortBindings = new[]
                {
                    new ContainerConfiguration.PortBinding
                    {
                        GuestTcpPort = 80, HostTcpPort = port
                    }
                },
                ContainerEnvironmentVariables = new[]
                {
                    $"ASPNETCORE_URLS=http://*:80",

                    // NOTE (Cameron): This is required for introspection within the container.
                    $"AUTHORITY=http://localhost",
                    $"IRONCLAD_CONNECTIONSTRING={connectionString}",

                    // NOTE (Cameron): Not secret. Only functional for development eg. localhost.
                    $"GOOGLE_CLIENT_ID=835517018777-4hnr0i9s8750kb10uaejdokel68bhtbb.apps.googleusercontent.com",
                    $"GOOGLE_SECRET=LCPH4fgebc-i4JR99GmoYU-X",
                },
            };

            this.probe = new IroncladProbe(authority, 2, 20);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);
            await this.probe.WaitUntilAvailable(true, default).ConfigureAwait(false);
        }
    }
}