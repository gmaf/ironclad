// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    public class ScopeModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Emphasize { get; set; }

        public bool Required { get; set; }

        public bool Checked { get; set; }
    }
}
