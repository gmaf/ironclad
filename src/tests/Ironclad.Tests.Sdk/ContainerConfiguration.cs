// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net;

#pragma warning disable CA1819

namespace Ironclad.Tests.Sdk
{
    using System;

    internal class ContainerConfiguration
    {
        // Image related
        public string Registry { get; set; }

        public NetworkCredential RegistryCredentials { get; set; }

        public string Image { get; set; }

        public string Tag { get; set; }

        public string TagQualifiedImage => this.Image + ":" + this.Tag;

        public string RegistryQualifiedImage => this.Registry != null ? this.Registry + "/" + this.Image : this.Image;

        public string FullyQualifiedImage => this.Registry != null ? this.Registry + "/" + this.TagQualifiedImage : this.TagQualifiedImage;

        // Container related
        public bool IsContainerReusable { get; set; }

        public string ContainerName { get; set; }

        public PortBinding[] ContainerPortBindings { get; set; } = Array.Empty<PortBinding>();

        public string[] ContainerEnvironmentVariables { get; set; } = Array.Empty<string>();

        public class PortBinding
        {
            public int GuestTcpPort { get; set; }

            public int HostTcpPort { get; set; }
        }
    }
}