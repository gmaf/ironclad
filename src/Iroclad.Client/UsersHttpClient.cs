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
        public Task<ResourceSet<UserSummary>> GetUserSummariesAsync(int start = default, int size = default, CancellationToken cancellationToken = default) =>
            this.GetAsync<ResourceSet<UserSummary>>(this.RelativeUrl($"{ApiPath}?skip={start}&take={(size == 0 ? 20 : size)}"), cancellationToken);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        public Task<User> GetUserAsync(string username, CancellationToken cancellationToken = default) =>
            this.GetAsync<User>(this.RelativeUrl($"{ApiPath}/{username}"), cancellationToken);

        /// <summary>
        /// Registers the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RegisterUserAsync(User user, CancellationToken cancellationToken = default) =>
            this.SendAsync<User>(HttpMethod.Post, this.RelativeUrl(ApiPath), user, cancellationToken);

        /// <summary>
        /// Modifies the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task ModifyUserAsync(User user, CancellationToken cancellationToken = default) =>
            this.SendAsync<User>(HttpMethod.Put, this.RelativeUrl($"{ApiPath}/{user?.Username}"), user, cancellationToken);
    }
}