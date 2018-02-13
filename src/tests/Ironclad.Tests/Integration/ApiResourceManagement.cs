// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Feature
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using FluentAssertions;
    using IdentityModel.Client;
    using Ironclad.Client;
    using Ironclad.Tests.Sdk;
    using Xunit;

    public class ApiResourceManagement : IntegrationTest
    {
        private const string Authority = "http://localhost:5005";

        public ApiResourceManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanAddApiResourceMinimum()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                ApiSecret = "secret",
            };

            // act
            await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetApiResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Name.Should().Be(expectedResource.Name);
        }

        [Fact]
        public async Task CanAddApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(ApiResourceManagement)}.{nameof(this.CanAddApiResource)} (integration test)",
                ApiSecret = "secret",
                UserClaims = { "name", "role" },
                ApiScopes = { new ApiResource.Scope { Name = "api", UserClaims = { "profile" } } },
                Enabled = false,
            };

            // act
            await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetApiResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Should().BeEquivalentTo(expectedResource, options => options.Excluding(resource => resource.ApiSecret));
        }

        [Fact]
        public async Task CanGetApiResourceSummaries()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(ApiResourceManagement)}.{nameof(this.CanGetApiResourceSummaries)} (integration test)",
                ApiSecret = "secret",
            };

            // act
            await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var resourceSummaries = await httpClient.GetApiResourceSummariesAsync().ConfigureAwait(false);
            resourceSummaries.Should().NotBeNull();
            resourceSummaries.Should().Contain(summary => summary.Name == expectedResource.Name && summary.DisplayName == expectedResource.DisplayName);
        }

        [Fact]
        public async Task CanModifyApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var originalApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(ApiResourceManagement)}.{nameof(this.CanModifyApiResource)} (integration test)",
                ApiSecret = "secret",
                UserClaims = { "name", "role" },
                ApiScopes = { new ApiResource.Scope { Name = "api", UserClaims = { "profile" } } },
                Enabled = false,
            };

            var expectedResource = new ApiResource
            {
                Name = originalApiResource.Name,
                DisplayName = $"{nameof(ApiResourceManagement)}.{nameof(this.CanModifyApiResource)} (integration test) #2",
                UserClaims = { "profile" },
                ApiScopes = { new ApiResource.Scope { Name = "test_api", UserClaims = { "name", "role" } } },
                Enabled = false,
            };

            await httpClient.AddApiResourceAsync(originalApiResource).ConfigureAwait(false);

            // act
            await httpClient.ModifyApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetApiResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Should().BeEquivalentTo(expectedResource, options => options.Excluding(resource => resource.ApiSecret));
        }

        [Fact]
        public async Task CanRemoveApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(ApiResourceManagement)}.{nameof(this.CanRemoveApiResource)} (integration test)",
                ApiSecret = "secret",
            };

            await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // act
            await httpClient.RemoveApiResourceAsync(expectedResource.Name).ConfigureAwait(false);

            // assert
            var resourceSummaries = await httpClient.GetApiResourceSummariesAsync().ConfigureAwait(false);
            resourceSummaries.Should().NotBeNull();
            resourceSummaries.Should().NotContain(summary => summary.Name == expectedResource.Name);
        }

        // LINK (Cameron): https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation/blob/dev/src/IdentityServer4.AccessTokenValidation/IdentityServerAuthenticationOptions.cs#L231
        [Fact]
        public async Task CanUseApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var resourcce = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                ApiSecret = "secret",
            };

            await httpClient.AddApiResourceAsync(resourcce).ConfigureAwait(false);

            // act
            var client = new IntrospectionClient(Authority + "/connect/introspect", resourcce.Name, resourcce.ApiSecret);
            var response = await client.SendAsync(new IntrospectionRequest { Token = "invalid" }).ConfigureAwait(false);

            // assert
            response.IsError.Should().BeFalse();
        }

        [Fact]
        public void CannotAddInvalidApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient(Authority);
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
            };

            // act
#pragma warning disable IDE0039
            Func<Task> func = async () => await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            func.Should().Throw<HttpException>();
        }
    }
}
