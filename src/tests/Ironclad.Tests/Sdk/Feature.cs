namespace Ironclad.Tests.Sdk
{
    using System;
    using Xunit;

    [Collection("Ironclad")]
    public class Feature
    {
        private IroncladFixture fixture;

        public Feature(IroncladFixture fixture)
        {
            this.fixture = fixture;
        }
    }
}
