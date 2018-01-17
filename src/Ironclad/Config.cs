// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.Test;

    internal static class Config
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

        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" },
                },
            };

        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "users_api",
                    DisplayName = "Users API",
                    Description = "Users API Access",
                    UserClaims = new List<string> { "role" },
                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    Scopes = new List<Scope>
                    {
                        new Scope("users_api.read"),
                        new Scope("users_api.write")
                    }
                }
            };

        public static List<TestUser> GetTestUsers() =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "af4ecfd06dc4489ead44c0b3aa639b11",
                    Username = "test",
                    Password = "test",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "test@lykke.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
    }
}
