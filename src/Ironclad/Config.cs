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
                // NOTE (Cameron): This is the sample client (console app; representing server-to-server communication).
                new Client
                {
                    ClientId = "sample_client",
                    ClientName = "Sample Client Application",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new List<string> { "sample_api.read", "sample_api.write" },
                    AccessTokenType = AccessTokenType.Reference,
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
                new ApiResource("sample_api", "Sample Web API")
                {
                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    Scopes = new List<Scope> { new Scope("sample_api.read"), new Scope("sample_api.write") }
                },
            };

        ////public static List<TestUser> GetTestUsers() =>
        ////    new List<TestUser>
        ////    {
        ////        new TestUser
        ////        {
        ////            SubjectId = "af4ecfd06dc4489ead44c0b3aa639b11",
        ////            Username = "sample",
        ////            Password = "sample",
        ////            Claims = new List<Claim>
        ////            {
        ////                new Claim(JwtClaimTypes.Email, "sample@lykke.com"),
        ////                new Claim(JwtClaimTypes.Role, "admin")
        ////            }
        ////        }
        ////    };
    }
}
