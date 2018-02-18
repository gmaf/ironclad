// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Feature
{
    using System;
    using System.Globalization;
    using System.Net;
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
            var httpClient = new RolesHttpClient(this.Authority, this.Handler);
            var role = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            // act
            await httpClient.AddRoleAsync(role).ConfigureAwait(false);

            // assert
            var roleExists = await httpClient.RoleExistsAsync(role).ConfigureAwait(false);
            roleExists.Should().BeTrue();
        }

        [Fact]
        public async Task CanGetRoleSummaries()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority, this.Handler);
            var role = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            // act
            await httpClient.AddRoleAsync(role).ConfigureAwait(false);

            // assert
            var roles = await httpClient.GetRolesAsync().ConfigureAwait(false);
            roles.Should().NotBeNull();
            roles.Should().Contain(role);
        }

        [Fact]
        public async Task CanRemoveRole()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority, this.Handler);
            var role = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            await httpClient.AddRoleAsync(role).ConfigureAwait(false);

            // act
            await httpClient.RemoveRoleAsync(role).ConfigureAwait(false);

            // assert
            var roles = await httpClient.GetRolesAsync().ConfigureAwait(false);
            roles.Should().NotBeNull();
            roles.Should().NotContain(role);
        }

        [Fact]
        public async Task CannotAddDuplicateRole()
        {
            // arrange
            var httpClient = new RolesHttpClient(this.Authority, this.Handler);
            var role = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            await httpClient.AddRoleAsync(role).ConfigureAwait(false);

            // act
            Func<Task> func = async () => await httpClient.AddRoleAsync(role).ConfigureAwait(false);

            // assert
            func.Should().Throw<HttpException>().And.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}
