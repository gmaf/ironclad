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

    public class UserManagement : IntegrationTest
    {
        public UserManagement(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CanRegisterUser()
        {
            // arrange
            var httpClient = new UsersHttpClient("http://localhost:5005");
            var expectedUser = new User
            {
                Username = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                Password = "password",
            };

            // act
            await httpClient.RegisterUserAsync(expectedUser).ConfigureAwait(false);

            // assert
            var userSummaries = await httpClient.GetUserSummariesAsync().ConfigureAwait(false);
            var actualUser = await httpClient.GetUserAsync(expectedUser.Username).ConfigureAwait(false);

            userSummaries.Should().NotBeNull();
            userSummaries.Should().Contain(clientSummary => clientSummary.Username == expectedUser.Username);
            actualUser.Should().NotBeNull();
            actualUser.Username.Should().Be(expectedUser.Username);
        }

        ////[Fact]
        ////public void CanModifyClient()
        ////{
        ////}

        ////[Fact]
        ////public void CanUnregisterClient()
        ////{
        ////}
    }
}
