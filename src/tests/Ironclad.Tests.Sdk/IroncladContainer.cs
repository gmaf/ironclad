// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Threading.Tasks;

    internal class IroncladContainer : Container
    {
        private readonly IroncladProbe probe;

        public IroncladContainer(string authority, int port, string connectionString, NetworkCredential registryCredentials)
        {
            authority = authority ?? throw new ArgumentNullException(nameof(authority));
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            registryCredentials = registryCredentials ?? throw new ArgumentNullException(nameof(registryCredentials));

            this.Configuration = new ContainerConfiguration
            {
                Registry = "lykkecloud.azurecr.io",
                RegistryCredentials = registryCredentials,
                Image = "ironclad",
                Tag = "dev",
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

            this.probe = new IroncladProbe(authority, 0, 20);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);
            await this.probe.WaitUntilAvailable(true, default).ConfigureAwait(false);
        }
    }
}