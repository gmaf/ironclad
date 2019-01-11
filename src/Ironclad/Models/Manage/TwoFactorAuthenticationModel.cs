// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    public class TwoFactorAuthenticationModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}
