// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Collections.Generic;
    using IdentityServer4;
    using IdentityServer4.Models;

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
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "sample_api" },
                    ////AccessTokenType = AccessTokenType.Reference,
                },

                // NOTE (Cameron): This is the sample client (console app; representing server-to-server communication).
                new Client
                {
                    ClientId = "spa",
                    ClientName = "Single Page Application",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { "http://localhost:5008/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5008/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5008" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        ////"role",
                        "sample_api",
                    },

                    AccessTokenType = AccessTokenType.Reference,
                }
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
                    UserClaims = new[] { "name", "role" }, // NOTE (Cameron): These are the user claims that are required by the web API.
                },
            };
    }
}
