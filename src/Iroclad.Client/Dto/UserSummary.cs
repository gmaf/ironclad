// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    /// <summary>
    /// Represents a user summary.
    /// </summary>
    public class UserSummary
    {
        /// <summary>
        /// Gets or sets the subject identifier for this user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the username for this user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public string Email { get; set; }
    }
}
