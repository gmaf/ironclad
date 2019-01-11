// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Controllers
{
    using System;

    public static class AccountOptions
    {
        public static readonly bool AllowLocalLogin = true;
        public static readonly bool AllowRememberLogin = true;
        public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static readonly bool ShowLogoutPrompt = true;
        public static readonly bool AutomaticRedirectAfterSignOut = false;
        public static readonly string InvalidCredentialsErrorMessage = "Invalid username or password";

        // specify the Windows authentication scheme being used
        public static readonly string WindowsAuthenticationSchemeName = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;

        // if user uses windows auth, should we load the groups from windows
        public static readonly bool IncludeWindowsGroups = false;
    }
}
