// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    // LINK (Cameron): https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/System/Net/Http/HttpClient.cs#L322
    internal static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> HeadAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
