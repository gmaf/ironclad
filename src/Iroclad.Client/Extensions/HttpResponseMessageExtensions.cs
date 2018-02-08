// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal static class HttpResponseMessageExtensions
    {
        public static async Task<HttpResponseMessage> EnsureSuccess(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask.ConfigureAwait(false);
            return await EnsureSuccess(response).ConfigureAwait(false);
        }

        // LINK (Cameron): https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs#L148
        public static async Task<HttpResponseMessage> EnsureSuccess(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var contentMessage = default(string);
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                contentMessage = JsonConvert.DeserializeObject<Content>(content)?.Message;

                response.Content.Dispose();
            }

            throw new HttpException(
                response.RequestMessage.Method,
                response.RequestMessage.RequestUri,
                response.StatusCode,
                response.ReasonPhrase,
                contentMessage);
        }

        private class Content
        {
            public string Message { get; set; }
        }
    }
}
