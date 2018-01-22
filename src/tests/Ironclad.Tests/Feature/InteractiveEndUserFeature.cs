namespace Ironclad.Tests.Feature
{
    using Ironclad.Tests.Sdk;
    using Xbehave;
    using Xunit;

    // As a user of the system
    // In order to access system data that is specific to me
    // I want to be able to sign-in to the system
    [TestCaseOrderer("Ironclad.Tests.Sdk.OrderStrategy", "Ironclad.Tests")]
    public class InteractiveEndUserFeature : Feature
    {

        public InteractiveEndUserFeature(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Scenario]
        public void CanSignIn(string username, string password)
        {
            "Given some hard-coded credentials eg. bob/bob"
                .x(() => { });

            "And some restricted data".x(() => { });

            "When I navigate to the secure [sample] website"
                .x(() =>
                {
                    // using BrowseR
                });

            "And I log in using the credentials".x(() => { });

            "Then I am able to view the restricted data".x(() => { });
        }

        [Scenario]
        public void CanSignOut(string username, string password)
        {
            "Given some hard-coded credentials eg. bob/bob"
                .x(() => { });

            "And some restricted data".x(() => { });

            "When I navigate to the secure [sample] website"
                .x(() =>
                {
                    // using BrowseR
                });

            "And I log in using the credentials".x(() => { });

            "Then I am able to view the restricted data".x(() => { });
        }
    }
}
 