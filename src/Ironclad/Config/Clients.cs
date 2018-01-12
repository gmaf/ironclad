// <copyright file="Clients.cs" company="Lykke">
// Copyright (c) Ironclad Contributors. All rights reserved.
// </copyright>

namespace Ironclad.Config
{
    using System.Collections.Generic;
    using IdentityServer4.Models;

    internal class Clients
    {
        public static IEnumerable<Client> GetInMemoryClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "oauthClient",
                    ClientName = "Example Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("superSecretPassword".Sha256())
                    },
                    AllowedScopes = new List<string> { "customAPI.read" },
                },
            };
    }
}
