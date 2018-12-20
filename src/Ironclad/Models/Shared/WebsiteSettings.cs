// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    public class WebsiteSettings
    {
        public string Styles { get; set; } = "css/default.css";

        public string Logo { get; set; } = "img/fingerprint.svg";

        public bool ShowLoginScreen => this.show_login_screen.HasValue ? this.show_login_screen.Value : true;

#pragma warning disable IDE1006, SA1300
        private bool? show_login_screen { get; set; }
    }
}
