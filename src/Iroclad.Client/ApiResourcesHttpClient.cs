// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An HTTP client for managing API resources for the authorization server.
    /// </summary>
    public sealed class ApiResourcesHttpClient : HttpClientBase, IApiResourcesClient
    {
        private const string ApiPath = "/api/apiresources";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourcesHttpClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public ApiResourcesHttpClient(string authority, HttpMessageHandler innerHandler = null)
            : base(authority, innerHandler)
        {
        }

        /// <summary>
        /// Gets the API resource summaries (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the resource set to return.</param>
        /// <param name="size">The total size of the resource set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource summaries.</returns>
        public Task<ResourceSet<ResourceSummary>> GetApiResourceSummariesAsync(int start = 0, int size = 0, CancellationToken cancellationToken = default) =>
            this.GetAsync<ResourceSet<ResourceSummary>>(this.RelativeUrl($"{ApiPath}?skip={start}&take={(size == 0 ? 20 : size)}"), cancellationToken);

        /// <summary>
        /// Gets the specified API resource.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource.</returns>
        public Task<ApiResource> GetApiResourceAsync(string resourceName, CancellationToken cancellationToken = default) =>
            this.GetAsync<ApiResource>(this.RelativeUrl($"{ApiPath}/{resourceName}"), cancellationToken);

        /// <summary>
        /// Adds the specified API resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task AddApiResourceAsync(ApiResource resource, CancellationToken cancellationToken = default) =>
            this.SendAsync<ApiResource>(HttpMethod.Post, this.RelativeUrl(ApiPath), resource, cancellationToken);

        /// <summary>
        /// Removes the specified API resource.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RemoveApiResourceAsync(string resourceName, CancellationToken cancellationToken = default) =>
            this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{this.SafeGetValue(resourceName, nameof(resourceName))}"), cancellationToken);

        /// <summary>
        /// Modifies the specified API resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task ModifyApiResourceAsync(ApiResource resource, CancellationToken cancellationToken = default) =>
            this.SendAsync<ApiResource>(HttpMethod.Put, this.RelativeUrl($"{ApiPath}/{this.SafeGetValue(resource?.Name, "resource.Name")}"), resource, cancellationToken);
    }
 }