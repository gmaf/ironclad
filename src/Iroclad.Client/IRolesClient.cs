// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the public members of the roles client.
    /// </summary>
    public interface IRolesClient
    {
        /// <summary>
        /// Gets the roles (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the role set to return.</param>
        /// <param name="size">The total size of the role set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The roles.</returns>
        Task<ResourceSet<string>> GetRolesAsync(int start = 0, int size = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the role exists.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <c>true</c> if the role exists; otherwise, <c>false</c>.</returns>
        Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task AddRoleAsync(string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        Task RemoveRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
