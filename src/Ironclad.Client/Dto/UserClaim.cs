// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    /// <summary>
    /// Represents a user claim.
    /// </summary>
    public class UserClaim
    {
        /// <summary>
        /// Gets or sets the type for this claim.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value for this claim.
        /// </summary>
        public string Value { get; set; }
    }
}