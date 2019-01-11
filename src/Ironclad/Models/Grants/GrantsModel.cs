// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    using System;
    using System.Collections.Generic;

    public class GrantsModel
    {
        public IList<Grant> Grants { get; } = new List<Grant>();

#pragma warning disable CA1034, CA1056
        public class Grant
        {
            public string ClientId { get; set; }

            public string ClientName { get; set; }

            public string ClientUrl { get; set; }

            public string ClientLogoUrl { get; set; }

            public DateTime Created { get; set; }

            public DateTime? Expires { get; set; }

            public IEnumerable<string> IdentityGrantNames { get; set; }

            public IEnumerable<string> ApiGrantNames { get; set; }
        }
    }
}
