// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;

    public class ExternalLoginsModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        public string StatusMessage { get; set; }
    }
}
