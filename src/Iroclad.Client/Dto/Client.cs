// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a client summary.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        /// <value>The client name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the allowed CORS origins.
        /// </summary>
        /// <value>The allowed CORS origins.</value>
#pragma warning disable CA2227
        public List<string> AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Gets or sets the redirect URIs.
        /// </summary>
        /// <value>The redirect URIs.</value>
        public List<string> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the post logout redirect URIs.
        /// </summary>
        /// <value>The post logout redirect URIs.</value>
        public List<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the allowed scopes.
        /// </summary>
        /// <value>The allowed scopes.</value>
        public List<string> AllowedScopes { get; set; }

        /// <summary>
        /// Gets or sets the type of the access token.
        /// </summary>
        /// <value>The type of the access token.</value>
        public string AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Client"/> is enabled.
        /// </summary>
        /// <value>Returns <c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool? Enabled { get; set; }
    }
}
