// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using System.Collections.Generic;

    public class ApiResource : Resource
    {
        public string Secret { get; set; }

        public ICollection<string> Scopes { get; set; }
    }
}
