// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    /// <summary>
    /// Represents a Resource summary.
    /// </summary>
    public class ResourceSummary
    {
        /// <summary>
        /// Gets or sets the Resource name.
        /// </summary>
        /// <value>The resource name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Resource display name.
        /// </summary>
        /// <value>The Resource display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Resource"/> is enabled.
        /// </summary>
        /// <value>Returns <c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
    }
}
