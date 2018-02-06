// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    using System.Collections.Generic;

#pragma warning disable CA1056
    public class ConsentModel : ConsentInputModel
    {
        public string ClientName { get; set; }

        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeModel> IdentityScopes { get; set; }

        public IEnumerable<ScopeModel> ResourceScopes { get; set; }
    }
}
