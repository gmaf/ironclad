// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Feature
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Ironclad.Client;
    using Ironclad.Tests.Sdk;
    using Xunit;

    public class ResourceManagement : IntegrationTest
    {
        public ResourceManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanAddApiResource()
        {
            // arrange
            var httpClient = new ApiResourcesHttpClient("http://localhost:5005");
            var expectedResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                DisplayName = "Test API Resource",
                ApiSecret = "secret",
                ////ApiScopes = { new ApiResource.Scope { Name = "api", UserClaims = { "name", "role" } } },
            };

            // act
            await httpClient.AddApiResourceAsync(expectedResource).ConfigureAwait(false);

            // assert
            var resourceSummaries = await httpClient.GetApiResourceSummariesAsync().ConfigureAwait(false);
            var actualResource = await httpClient.GetApiResourceAsync(expectedResource.Name).ConfigureAwait(false);

            resourceSummaries.Should().NotBeNull();
            resourceSummaries.Should().Contain(resourceSummary => resourceSummary.Name == expectedResource.Name && resourceSummary.DisplayName == expectedResource.DisplayName);
            actualResource.Should().NotBeNull();
            actualResource.Name.Should().Be(expectedResource.Name);
            actualResource.DisplayName.Should().Be(expectedResource.DisplayName);
        }

        [Fact]
        public void CanModifyApiResource()
        {
        }

        [Fact]
        public void CanRemoveApiResource()
        {
        }
    }
}
