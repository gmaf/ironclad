// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An HTTP client for managing roles for users of the authorization server.
    /// </summary>
    public sealed class RolesHttpClient : HttpClientBase,  IRolesClient
    {
        private const string ApiPath = "/api/roles";

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesHttpClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public RolesHttpClient(string authority, HttpMessageHandler innerHandler = null)
            : base(authority, innerHandler)
        {
        }

        /// <summary>
        /// Gets the roles (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the role set to return.</param>
        /// <param name="size">The total size of the role set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The roles.</returns>
        public async Task<ResourceSet<string>> GetRolesAsync(int start = 0, int size = 20, CancellationToken cancellationToken = default)
        {
            var resourceSet = await this.GetAsync<ResourceSet<Role>>(
                this.RelativeUrl($"{ApiPath}?skip={Valid(start, nameof(start))}&take={Valid(size, nameof(size))}"),
                cancellationToken)
                .ConfigureAwait(false);

            return new ResourceSet<string>(resourceSet.Start, resourceSet.TotalSize, resourceSet.Select(role => role.Name));
        }

        /// <summary>
        /// Checks the role exists.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <c>true</c> if the role exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default)
        {
            var url = this.RelativeUrl($"/api/roles/{Valid(role, nameof(role))}");

            try
            {
                using (var response = await this.Client.HeadAsync(url, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }

                    await response.EnsureSuccess().ConfigureAwait(false);

                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(HttpMethod.Head, new Uri(url), ex);
            }
        }

        /// <summary>
        /// Adds the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task AddRoleAsync(string role, CancellationToken cancellationToken = default) =>
            this.SendAsync<Role>(HttpMethod.Post, this.RelativeUrl(ApiPath), new Role { Name = Valid(role, nameof(role)) }, cancellationToken);

        /// <summary>
        /// Removes the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RemoveRoleAsync(string role, CancellationToken cancellationToken = default) =>
            this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{Valid(role, nameof(role))}"), cancellationToken);
    }
}