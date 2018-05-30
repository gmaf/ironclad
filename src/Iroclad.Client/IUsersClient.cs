// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the public members of the users client.
    /// </summary>
    public interface IUsersClient
    {
        /// <summary>
        /// Get the user summaries (or a subset thereof).
        /// </summary>
        /// <param name="startsWith">The start of the username.</param>
        /// <param name="start">The zero-based start ordinal of the user set to return.</param>
        /// <param name="size">The total size of the user set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user summaries.</returns>
        Task<ResourceSet<UserSummary>> GetUserSummariesAsync(string startsWith = default, int start = 0, int size = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        Task<User> GetUserAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sendEmail"> (default true) Determines whether to send a confirmation email or not.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new user.</returns>
        Task<NewUser> AddUserAsync(User user, bool sendEmail = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task RemoveUserAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="currentUsername">The current username (if different from the specified user username).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified user.</returns>
        Task<User> ModifyUserAsync(User user, string currentUsername = null, CancellationToken cancellationToken = default);
    }
}
