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

    public class RoleManagament : IntegrationTest
    {
        public RoleManagament(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanAddRole()
        {
            // arrange
            var httpClient = new RolesHttpClient("http://localhost:5005");
            var expectedRole = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            // act
            await httpClient.AddRoleAsync(expectedRole).ConfigureAwait(false);

            // assert
            var roles = await httpClient.GetRolesAsync().ConfigureAwait(false);
            var roleExists = await httpClient.RoleExistsAsync(expectedRole).ConfigureAwait(false);

            roles.Should().NotBeNull();
            roles.Should().Contain(role => role.Name == expectedRole);
            roleExists.Should().BeTrue();
        }

        [Fact]
        public void CanRemoveRole()
        {
        }
    }
}
