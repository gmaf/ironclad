namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class HttpClientExtensions
    {
        /// <summary>
        /// Sends the http patch request on the specified url.
        /// </summary>
        /// <param name="client">Client on which the request is being sent.</param>
        /// <param name="requestUri">The url on which to send the patch request</param>
        /// <param name="content">The http content to send</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The http response message</returns>
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the http delete request on the specified url.
        /// </summary>
        /// <param name="client">Client on which the request is being sent.</param>
        /// <param name="requestUri">The url on which to send the delete request</param>
        /// <param name="content">The http content to send</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("DELETE");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
