// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1054

namespace Ironclad.Client
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Represents the base class for the HTTP clients.
    /// </summary>
    public class HttpClientBase : IDisposable
    {
        private static readonly JsonSerializerSettings Settings = GetJsonSerializerSettings();

        private readonly string authority;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientBase"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public HttpClientBase(string authority, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            this.Client = new HttpClient(handler);
            this.authority = authority;
        }

        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        /// <value>The HTTP client.</value>
        protected HttpClient Client { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET operation.
        /// </summary>
        /// <typeparam name="T">The type of data transfer object.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The data transfer object.</returns>
        protected async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
        {
            this.EnsureNotDisposed();

            var content = default(string);
            try
            {
                using (var response = await this.Client.GetAsync(requestUri, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Get, new Uri(requestUri), ex);
            }

            return JsonConvert.DeserializeObject<T>(content, Settings);
        }

        /// <summary>
        /// Performs an asynchronous HTTP operation.
        /// </summary>
        /// <typeparam name="T">The type of data transfer object.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        protected async Task SendAsync<T>(HttpMethod method, string requestUri, T resource, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var content = new StringContent(JsonConvert.SerializeObject(resource, Settings), Encoding.UTF8, "application/json"))
                using (var request = new HttpRequestMessage(method, requestUri) { Content = content })
                using (var response = await this.Client.SendAsync(request, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(method, new Uri(requestUri), ex);
            }
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE operation.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        protected async Task DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var response = await this.Client.DeleteAsync(requestUri, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Delete, new Uri(requestUri), ex);
            }
        }

        /// <summary>
        /// Returns a URL relative to the authority.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A URL.</returns>
#pragma warning disable CA1055
        protected string RelativeUrl(string path) => this.authority + path;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">Set to <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                this.Client.Dispose();
            }
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        private void EnsureNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
