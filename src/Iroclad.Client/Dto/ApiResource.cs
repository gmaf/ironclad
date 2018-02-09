// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Api Resource.
    /// </summary>
    public class ApiResource : Resource
    {
        /// <summary>
        /// Gets or sets secret for this api resource.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets scopes for this api resource.
        /// </summary>
        public ICollection<string> Scopes { get; set; }
    }
}
