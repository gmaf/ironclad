// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the public members of the Ironclad client.
    /// </summary>
    public interface IIroncladClient
    {
        /// <summary>
        /// Gets the client summaries (or a subset thereof).
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the client set to return.</param>
        /// <param name="size">The total size of the client set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The client summaries.</returns>
        Task<ResourceSet<ClientSummary>> GetClientSummariesAsync(int start = default, int size = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The client.</returns>
        Task<Client> GetClientAsync(string clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task RegisterClientAsync(Client client, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unregisters the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task UnregisterClientAsync(Client client, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task ModifyClientAsync(Client client, CancellationToken cancellationToken = default);

        /*
        // delete
        Task UnregisterClientAsync(string clientId, CancellationToken cancellationToken = default);
        */

        /// <summary>
        /// Get the user summaries (or a subset thereof)
        /// </summary>
        /// <param name="start">The zero-based start ordinal of the user set to return.</param>
        /// <param name="size">The total size of the user set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user summaries.</returns>
        Task<ResourceSet<UserSummary>> GetUserSummariesAsync(int start = default, int size = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the user
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task RegisterUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unregisters the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task UnregisterUserAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task ModifyUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes password for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="currentPassword">Current password of the user.</param>
        /// <param name="newPassword">New password of the user</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets roles assigned to the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of roles assigned to the user.</returns>
        Task<ResourceSet<Role>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns specified roles to the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">List of role name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task AssignRolesToUserAsync(string userId, List<string> roles, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes specified user from the given roles.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">List of role name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task UnassignRolesFromUserAsync(string userId, List<string> roles, CancellationToken cancellationToken = default);
    }
}