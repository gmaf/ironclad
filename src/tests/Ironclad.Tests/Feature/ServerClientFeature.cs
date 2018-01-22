namespace Ironclad.Tests.Feature
{
    using IdentityModel.Client;
    using Ironclad.Tests.Sdk;
    using Xbehave;

    public class ServerClientFeature : Feature
    {
        public ServerClientFeature(IroncladFixture fixture)
            : base(fixture)
        {
        }

        [Scenario]
        public void CanCallSecureWebApi(string clientId, string secret)
        {
            "Given some hard-coded credentials eg. bob/bob"
                .x(async () =>
                {
                    var discoveryClient = new DiscoveryClient("http://localhost:5005");
                    var discoveryResponse = await discoveryClient.GetAsync();
                });

            "And some restricted data".x(
                () => 
                {
                });

            "When I navigate to the secure [sample] website"
                .x(() =>
                {
                });

            "And I log in using the credentials".x(() => { });

            "Then I am able to view the restricted data".x(() => { });
        }
    }
}
