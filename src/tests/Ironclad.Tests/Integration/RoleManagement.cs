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

    public class RoleManagement : IntegrationTest
    {
        public RoleManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanAddRole()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority);
            var expectedRole = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            // act
            await httpClient.AddRoleAsync(expectedRole).ConfigureAwait(false);

            // assert
            var roleExists = await httpClient.RoleExistsAsync(expectedRole).ConfigureAwait(false);
            roleExists.Should().BeTrue();
        }

        [Fact]
        public async Task CanGetRoleSummaries()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority);
            var expectedRole = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            // act
            await httpClient.AddRoleAsync(expectedRole).ConfigureAwait(false);

            // assert
            var roles = await httpClient.GetRolesAsync().ConfigureAwait(false);
            roles.Should().NotBeNull();
            roles.Should().Contain(expectedRole);
        }

        [Fact]
        public async Task CanRemoveRole()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority);
            var expectedRole = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            await httpClient.AddRoleAsync(expectedRole).ConfigureAwait(false);

            // act
            await httpClient.RemoveRoleAsync(expectedRole).ConfigureAwait(false);

            // assert
            var roles = await httpClient.GetRolesAsync().ConfigureAwait(false);
            roles.Should().NotBeNull();
            roles.Should().NotContain(expectedRole);
        }
    }
}
