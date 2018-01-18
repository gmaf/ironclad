namespace SampleClient
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var discoveryClient = new DiscoveryClient("http://localhost:5005"))
            {
                var discoveryResponse = await discoveryClient.GetAsync();

                using (var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "sample_client", "secret"))
                {
                    var tokenResponse = await tokenClient.RequestClientCredentialsAsync("sample_api.read");

                    // NOTE (Cameron): Remember to check the manner in which each instance of an HTTP client is used (see link below).
                    // LINK (Cameron): https://stackoverflow.com/questions/37157164/best-practice-for-use-httpclient
                    using (var apiClient = new HttpClient())
                    {
                        apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                        using (var apiResponse = await apiClient.GetAsync("http://localhost:5007/api"))
                        {
                            var content = apiResponse.Content.ReadAsStringAsync();
                            Console.WriteLine(content);
                        }
                    }
                }
            }
        }
    }
}
