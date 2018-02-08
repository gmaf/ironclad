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

    public class ClientManagement : IntegrationTest
    {
        public ClientManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanRegisterClient()
        {
            // arrange
            var httpClient = new IroncladClient("http://localhost:5005");
            var expectedClient = new Client
            {
                Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                Name = "Test Client",
            };

            // act
            await httpClient.RegisterClientAsync(expectedClient).ConfigureAwait(false);

            // assert
            var clientSummaries = await httpClient.GetClientSummariesAsync().ConfigureAwait(false);
            var actualClient = await httpClient.GetClientAsync(expectedClient.Id).ConfigureAwait(false);

            clientSummaries.Should().NotBeNull();
            clientSummaries.Should().Contain(clientSummary => clientSummary.Id == expectedClient.Id && clientSummary.Name == expectedClient.Name);
            actualClient.Should().NotBeNull();
            actualClient.Id.Should().Be(expectedClient.Id);
            actualClient.Name.Should().Be(expectedClient.Name);
        }

        [Fact]
        public void CanModifyClient()
        {
        }

        [Fact]
        public void CanUnregisterClient()
        {
        }
    }
}
