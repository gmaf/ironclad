// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Application
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }

#pragma warning disable CA1056
        public string AvatarUrl { get; set; }
    }
}
