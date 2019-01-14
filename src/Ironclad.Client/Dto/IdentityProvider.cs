// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#if PERSISTENCE
namespace Ironclad.ExternalIdentityProvider.Persistence
{
    using System.Collections.Generic;
    using Marten.Schema;
#else
namespace Ironclad.Client
{
    using System.Collections.Generic;
#endif

    /// <summary>
    /// Represents an identity provider.
    /// </summary>
    public class IdentityProvider
    {
        /// <summary>
        /// Gets or sets the name of the identity provider.
        /// </summary>
        /// <value>The name.</value>
#if PERSISTENCE
        [Identity]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name for the identity provider.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the authority for the identity provider.
        /// </summary>
        /// <value>The secret.</value>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the client identity for the identity provider.
        /// </summary>
        /// <value>The secret.</value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the callback path for the identity provider.
        /// </summary>
        /// <value>The secret.</value>
        public string CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the authentication context class reference values for the identity provider.
        /// </summary>
        /// <value>Space-separated string that specifies the authentication context class reference values.</value>
        public ICollection<string> AcrValues { get; set; }
#if CLIENT
            = new HashSet<string>();
#endif

        /// <summary>
        /// Gets or sets the scopes to use for the identity provider.
        /// </summary>
        /// <value>The scopes.</value>
        public ICollection<string> Scopes { get; set; }
#if CLIENT
            = new HashSet<string>();
#endif

        /// <summary>
        /// Gets or sets a value indicating whether or not to auto-provision the user without prompting for further information.
        /// </summary>
        /// <value>Returns <c>true</c> if auto-provisioning is configured; otherwise, <c>false</c>.</value>
        public bool? AutoProvision { get; set; }
    }
}
