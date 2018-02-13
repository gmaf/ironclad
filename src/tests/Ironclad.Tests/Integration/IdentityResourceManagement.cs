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

    public class IdentityResourceManagement : IntegrationTest
    {
        public IdentityResourceManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanAddIdentityResourceMinimum()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var expectedResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                UserClaims = { "role" },
            };

            // act
            await httpClient.AddIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetIdentityResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Name.Should().Be(expectedResource.Name);
            actualResource.UserClaims.Should().Contain(expectedResource.UserClaims);
        }

        [Fact]
        public async Task CanAddIdentityResource()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var expectedResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(IdentityResourceManagement)}.{nameof(this.CanAddIdentityResource)} (integration test)",
                UserClaims = { "name", "role" },
                Enabled = false,
            };

            // act
            await httpClient.AddIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetIdentityResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Should().BeEquivalentTo(expectedResource);
        }

        [Fact]
        public async Task CanGetIdentityResourceSummaries()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var expectedResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(IdentityResourceManagement)}.{nameof(this.CanGetIdentityResourceSummaries)} (integration test)",
                UserClaims = { "role" },
            };

            // act
            await httpClient.AddIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var resourceSummaries = await httpClient.GetIdentityResourceSummariesAsync().ConfigureAwait(false);
            resourceSummaries.Should().NotBeNull();
            resourceSummaries.Should().Contain(summary => summary.Name == expectedResource.Name && summary.DisplayName == expectedResource.DisplayName);
        }

        [Fact]
        public async Task CanModifyIdentityResource()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var originalIdentityResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(IdentityResourceManagement)}.{nameof(this.CanModifyIdentityResource)} (integration test)",
                UserClaims = { "name", "role" },
                Enabled = false,
            };

            var expectedResource = new IdentityResource
            {
                Name = originalIdentityResource.Name,
                DisplayName = $"{nameof(IdentityResourceManagement)}.{nameof(this.CanModifyIdentityResource)} (integration test) #2",
                UserClaims = { "profile" },
                Enabled = false,
            };

            await httpClient.AddIdentityResourceAsync(originalIdentityResource).ConfigureAwait(false);

            // act
            await httpClient.ModifyIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var actualResource = await httpClient.GetIdentityResourceAsync(expectedResource.Name).ConfigureAwait(false);
            actualResource.Should().NotBeNull();
            actualResource.Should().BeEquivalentTo(expectedResource);
        }

        [Fact]
        public async Task CanRemoveIdentityResource()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var expectedResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = $"{nameof(IdentityResourceManagement)}.{nameof(this.CanRemoveIdentityResource)} (integration test)",
                UserClaims = { "role" },
            };

            await httpClient.AddIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // act
            await httpClient.RemoveIdentityResourceAsync(expectedResource.Name).ConfigureAwait(false);

            // assert
            var resourceSummaries = await httpClient.GetIdentityResourceSummariesAsync().ConfigureAwait(false);
            resourceSummaries.Should().NotBeNull();
            resourceSummaries.Should().NotContain(summary => summary.Name == expectedResource.Name);
        }

        [Fact]
        public void CannotAddInvalidIdentityResource()
        {
            // arrange
            var httpClient = new IdentityResourcesHttpClient(this.Authority);
            var expectedResource = new IdentityResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
            };

            // act
#pragma warning disable IDE0039
            Func<Task> func = async () => await httpClient.AddIdentityResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            func.Should().Throw<HttpException>();
        }
    }
}
