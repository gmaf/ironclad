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
        /// <param name="start">The zero-based start ordinal of the user set to return.</param>
        /// <param name="size">The total size of the user set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user summaries.</returns>
        Task<ResourceSet<UserSummary>> GetUserSummariesAsync(int start = default, int size = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        Task<User> GetUserAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task RegisterUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task ModifyUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
