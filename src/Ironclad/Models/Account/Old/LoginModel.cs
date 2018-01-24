// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models.Old
{
    using System.Collections.Generic;
    using System.Linq;

    public class LoginModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }

        public bool EnableLocalLogin { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }

        public IEnumerable<ExternalProvider> VisibleExternalProviders => this.ExternalProviders?.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders?.Count() == 1;

        public string ExternalLoginScheme => this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

#pragma warning disable CA1034
        public class ExternalProvider
        {
            public string DisplayName { get; set; }

            public string AuthenticationScheme { get; set; }
        }
    }
}