// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

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
    /// An HTTP client for managing clients of the Ironclad server.
    /// </summary>
    public sealed class IroncladClient : IIroncladClient, IDisposable
    {
        private static readonly JsonSerializerSettings Settings = GetJsonSerializerSettings();

        private readonly HttpClient client;
        private readonly string authority;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IroncladClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public IroncladClient(string authority, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            this.client = new HttpClient(handler);
            this.authority = authority;
        }

        /// <summary>
        /// Gets the clients (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the client set to return.</param>
        /// <param name="size">The total size of the client set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The clients.</returns>
        public async Task<ResourceSet<ClientSummary>> GetClientSummariesAsync(
            int start = default,
            int size = default,
            CancellationToken cancellationToken = default)
        {
            var url = this.authority + $"/api/clients?skip={start}&take={(size == 0 ? 20 : size)}";

            var content = default(string);
            try
            {
                using (var response = await this.client.GetAsync(url, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Get, new Uri(url), ex);
            }

            return JsonConvert.DeserializeObject<ResourceSet<ClientSummary>>(content, Settings);
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The client.</returns>
        public async Task<Client> GetClientAsync(string clientId, CancellationToken cancellationToken = default)
        {
            var url = this.authority + $"/api/clients/{clientId}";

            var content = default(string);
            try
            {
                using (var response = await this.client.GetAsync(url, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Get, new Uri(url), ex);
            }

            return JsonConvert.DeserializeObject<Client>(content, Settings);
        }

        /// <summary>
        /// Registers the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        public async Task RegisterClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            var url = this.authority + $"/api/clients";

            try
            {
                using (var httpContent = new StringContent(JsonConvert.SerializeObject(client, Settings), Encoding.UTF8, "application/json"))
                using (var response = await this.client.PostAsync(url, httpContent, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Post, new Uri(url), ex);
            }
        }

        /// <summary>
        /// Unregisters the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        public async Task UnregisterClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            var url = this.authority + $"/api/clients/{client.Id}";

            try
            {
                using (var response = await this.client.DeleteAsync(url, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Delete, new Uri(url), ex);
            }
        }

        /// <summary>
        /// Modifies the client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        public async Task ModifyClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            var url = this.authority + $"/api/clients/{client.Id}";

            try
            {
                using (var httpContent = new StringContent(JsonConvert.SerializeObject(client, Settings), Encoding.UTF8, "application/json"))
                using (var response = await this.client.PutAsync(url, httpContent, cancellationToken).EnsureSuccess().ConfigureAwait(false))
                {
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Put, new Uri(url), ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.client.Dispose();
            this.disposed = true;
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
    }
}