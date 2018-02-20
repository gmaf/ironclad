// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An HTTP client for managing users of the authorization server.
    /// </summary>
    public sealed class UsersHttpClient : HttpClientBase, IUsersClient
    {
        private const string ApiPath = "/api/users";

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersHttpClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public UsersHttpClient(string authority, HttpMessageHandler innerHandler = null)
            : base(authority, innerHandler)
        {
        }

        /// <summary>
        /// Get the user summaries (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the user set to return.</param>
        /// <param name="size">The total size of the user set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user summaries.</returns>
        public Task<ResourceSet<UserSummary>> GetUserSummariesAsync(int start = 0, int size = 20, CancellationToken cancellationToken = default) =>
            this.GetAsync<ResourceSet<UserSummary>>(this.RelativeUrl($"{ApiPath}?skip={Valid(start, nameof(start))}&take={Valid(size, nameof(size))}"), cancellationToken);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        public Task<User> GetUserAsync(string username, CancellationToken cancellationToken = default) =>
            this.GetAsync<User>(this.RelativeUrl($"{ApiPath}/{username}"), cancellationToken);

        /// <summary>
        /// Adds the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new user.</returns>
        public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await this.SendAsync<User>(HttpMethod.Post, this.RelativeUrl(ApiPath), user, cancellationToken).ConfigureAwait(false);
            return await this.GetUserAsync(user.Username, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RemoveUserAsync(string username, CancellationToken cancellationToken = default) =>
            this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{Valid(username, nameof(username))}"), cancellationToken);

        /// <summary>
        /// Modifies the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="currentUsername">The current username (if different from the specified user username).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified user.</returns>
        public async Task<User> ModifyUserAsync(User user, string currentUsername = null, CancellationToken cancellationToken = default)
        {
            var username = currentUsername ?? Valid(user?.Username, "user.Username");

            await this.SendAsync<User>(HttpMethod.Put, this.RelativeUrl($"{ApiPath}/{username}"), user, cancellationToken).ConfigureAwait(false);
            return await this.GetUserAsync(user.Username, cancellationToken).ConfigureAwait(false);
        }
    }
}