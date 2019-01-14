// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Configuration
{
    using Ironclad.Application;

    public static partial class Config
    {
        public const string DefaultAdminUserId = "e4744f59155040599fb057d62e84c079";

        public static ApplicationUser GetDefaultAdminUser() => new ApplicationUser { Id = DefaultAdminUserId, UserName = "admin" };
    }
}
