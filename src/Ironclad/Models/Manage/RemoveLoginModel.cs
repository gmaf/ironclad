// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    public class RemoveLoginModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
