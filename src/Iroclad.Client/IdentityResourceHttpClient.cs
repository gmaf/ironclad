// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An HTTP client for managing identity resources for the authorization server.
    /// </summary>
    public sealed class IdentityResourceHttpClient : HttpClientBase, IIdentityResourcesClient
    {
        private const string ApiPath = "/api/identityresources";

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceHttpClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public IdentityResourceHttpClient(string authority, HttpMessageHandler innerHandler = null)
            : base(authority, innerHandler)
        {
        }

        /// <summary>
        /// Gets the identity resource summaries (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the resource set to return.</param>
        /// <param name="size">The total size of the resource set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource summaries.</returns>
        public Task<ResourceSet<ResourceSummary>> GetIdentityResourceSummariesAsync(int start = 0, int size = 0, CancellationToken cancellationToken = default) =>
                    this.GetAsync<ResourceSet<ResourceSummary>>(this.RelativeUrl($"{ApiPath}?skip={start}&take={(size == 0 ? 20 : size)}"), cancellationToken);

        /// <summary>
        /// Gets the identity resource.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource.</returns>
        public Task<IdentityResource> GetIdentityResourceAsync(string resourceName, CancellationToken cancellationToken = default) =>
                    this.GetAsync<IdentityResource>(this.RelativeUrl($"{ApiPath}/{resourceName}"), cancellationToken);

        /// <summary>
        /// Adds the specified identity resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task AddIdentityResourceAsync(IdentityResource resource, CancellationToken cancellationToken = default) =>
                    this.SendAsync<IdentityResource>(HttpMethod.Post, this.RelativeUrl(ApiPath), resource, cancellationToken);

        /// <summary>
        /// Removes the specified identity resource.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RemoveIdentityResourceAsync(string resourceName, CancellationToken cancellationToken = default) =>
                    this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{resourceName}"), cancellationToken);

        /// <summary>
        /// Modifies the specified identity resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task ModifyIdentityResourceAsync(IdentityResource resource, CancellationToken cancellationToken = default) =>
                    this.SendAsync<IdentityResource>(HttpMethod.Put, this.RelativeUrl($"{ApiPath}/{resource?.Name}"), resource, cancellationToken);
    }
}