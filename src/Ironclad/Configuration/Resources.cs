// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Configuration
{
    using System.Collections.Generic;
    using IdentityServer4.Models;

    public static partial class Config
    {
        public static IEnumerable<IdentityResource> GetDefaultIdentityResources() =>
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

        // NOTE (Cameron): User claims defined against API resources will result in those claims being in the access token.
        public static IEnumerable<ApiResource> GetDefaultApiResources() =>
            new List<ApiResource>
            {
                new ApiResource("sample_api", "Sample Web API")
                {
                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    UserClaims = new[] { "name", "role" },
                },
                new ApiResource("template_api", "Lykke Web API (Template)")
                {
                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    UserClaims = new[] { "name", "role" },
                },
                new ApiResource("auth_api", "Authorization Server Web API")
                {
                    UserClaims = new[] { "name", "role" },
                },
            };
    }
}
