// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

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
        /// <param name="startsWith">The start of the username.</param>
        /// <param name="start">The zero-based start ordinal of the user set to return.</param>
        /// <param name="size">The total size of the user set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user summaries.</returns>
        public Task<ResourceSet<UserSummary>> GetUserSummariesAsync(
            string startsWith = default,
            int start = 0,
            int size = 20,
            CancellationToken cancellationToken = default) =>
            this.GetAsync<ResourceSet<UserSummary>>(
                this.RelativeUrl($"{ApiPath}?username={WebUtility.UrlEncode(startsWith)}&skip={NotNegative(start, nameof(start))}&take={NotNegative(size, nameof(size))}"),
                cancellationToken);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        public Task<User> GetUserAsync(string username, CancellationToken cancellationToken = default) =>
            this.GetAsync<User>(this.RelativeUrl($"{ApiPath}/{WebUtility.UrlEncode(NotNull(username, nameof(username)))}"), cancellationToken);

        /// <summary>
        /// Adds the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sendEmail"> (default true) Determines whether to send a confirmation email or not.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new user.</returns>
        public async Task<NewUser> AddUserAsync(User user, bool sendEmail = true, CancellationToken cancellationToken = default)
        {
            using (var httpResponse = await this.SendAsyncGetResponse<User>(HttpMethod.Post, this.RelativeUrl($"{ApiPath}?sendEmail={sendEmail}"), user, cancellationToken).ConfigureAwait(false))
            {
                string registrationLink = null;

                var response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response))
                {
                    var addUserResponse = JsonConvert.DeserializeObject<AddUserResponse>(response);
                    registrationLink = addUserResponse.RegistrationLink;
                }

                var userInTheSystem = await this.GetUserAsync(user.Username, cancellationToken).ConfigureAwait(false);
                var newUser = new NewUser(userInTheSystem, registrationLink);

                return newUser;
            }
        }

        /// <summary>
        /// Removes the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public Task RemoveUserAsync(string username, CancellationToken cancellationToken = default) =>
            this.DeleteAsync(this.RelativeUrl($"{ApiPath}/{WebUtility.UrlEncode(NotNull(username, nameof(username)))}"), cancellationToken);

        /// <summary>
        /// Modifies the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="currentUsername">The current username (if different from the specified user username).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified user.</returns>
        public async Task<User> ModifyUserAsync(User user, string currentUsername = null, CancellationToken cancellationToken = default)
        {
            var url = this.RelativeUrl($"{ApiPath}/{WebUtility.UrlEncode(currentUsername ?? NotNull(user?.Username, "user.Username"))}");
            await this.SendAsync<User>(HttpMethod.Put, url, user, cancellationToken).ConfigureAwait(false);
            return await this.GetUserAsync(user.Username, cancellationToken).ConfigureAwait(false);
        }
    }
}