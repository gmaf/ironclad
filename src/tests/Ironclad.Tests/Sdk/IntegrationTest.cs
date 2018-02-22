// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System.Net.Http;
    using Xunit;

    [Collection("Ironclad")]
    public class IntegrationTest
    {
        private IroncladFixture fixture;

        public IntegrationTest(IroncladFixture fixture)
        {
            this.fixture = fixture;
        }

        protected string Authority => this.fixture.Authority;

        protected HttpMessageHandler Handler => this.fixture.Handler;
    }
}
