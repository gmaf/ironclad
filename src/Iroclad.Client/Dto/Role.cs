// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    /// <summary>
    /// Represents a user role.
    /// </summary>
#if CLIENT
    internal class Role
#else
    public class Role
#endif
    {
        /// <summary>
        /// Gets or sets the name of this role.
        /// </summary>
        public string Name { get; set; }
    }
}
