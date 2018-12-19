// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Configuration
{
    using System.Collections.Generic;
    using IdentityModel;
    using IdentityServer4.Models;

    public static partial class Config
    {
        public static IEnumerable<IdentityResource> GetDefaultIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResource
                {
                    Description = "Your assigned role(s)",
                    Name = JwtClaimTypes.Role,
                    UserClaims = { JwtClaimTypes.Role },
                    ShowInDiscoveryDocument = false,
                },
            };

        // NOTE (Cameron): User claims defined against API resources will result in those claims being in the access token.
        public static IEnumerable<ApiResource> GetDefaultApiResources(string authApiSecret) =>
            new List<ApiResource>
            {
                new ApiResource("sample_api", "Sample Web API")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role },
                },
                new ApiResource("template_api", "Lykke Web API (Template)")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role },
                },
                new ApiResource("auth_api", "Authorization Server Web API")
                {
                    ApiSecrets = { new Secret(authApiSecret.Sha256()) },
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role },
                    Scopes = { new Scope("auth_api:write") },
                },
            };
    }
}
