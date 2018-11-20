// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Configuration
{
    using System.Collections.Generic;
    using IdentityServer4.Models;
    using Microsoft.Extensions.Configuration;

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
                    Name = "role",
                    UserClaims = { "role" },
                },
            };

        // NOTE (Cameron): User claims defined against API resources will result in those claims being in the access token.
        public static IEnumerable<ApiResource> GetDefaultApiResources(IConfiguration configuration) =>
            new List<ApiResource>
            {
                new ApiResource("sample_api", "Sample Web API")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    UserClaims = { "name", "role" },
                },
                new ApiResource("template_api", "Lykke Web API (Template)")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    UserClaims = { "name", "role" },
                },
                new ApiResource("auth_api", "Authorization Server Web API")
                {
                    ApiSecrets = { new Secret(configuration.GetValue<string>("Introspection-Secret").Sha256()) },
                    UserClaims = { "name", "role" },
                    Scopes = { new Scope("auth_api:write") },
                },
            };
    }
}
