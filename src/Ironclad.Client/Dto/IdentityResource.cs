// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a resource (either an API resource or an identity resource).
    /// </summary>
    //// NOTE (Cameron): This does not represent any one type of resource (eg. API or identity) but should be considered a superset of both from a DTO perspective only.
    public class IdentityResource
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name for the resource.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user claims for the resource.
        /// </summary>
        /// <value>The user claims.</value>
#pragma warning disable CA2227
        public ICollection<string> UserClaims { get; set; }
#if CLIENT
            = new HashSet<string>();
#endif

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IdentityResource"/> is enabled.
        /// </summary>
        /// <value>Returns <c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool? Enabled { get; set; }
    }
}
