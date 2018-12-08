// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

// TODO (Cameron): Refactor and remove when JSON configuration supports snake case.
#pragma warning disable IDE1006, SA1300

namespace Ironclad
{
    using System;

    internal class Settings
    {
        public ServerSettings Server { get; set; }

        public ApiSettings Api { get; set; }

        public IdpSettings Idp { get; set; }

        public MailSettings Mail { get; set; }

        public void Validate()
        {
            if (this.Api == null || this.Server == null)
            {
                // TODO (Cameron): Change link to point to somewhere sensible (when it exists).
                throw new InvalidOperationException(
                    "Validation of settings failed. Missing server or API sections. Please see: https://gist.github.com/cameronfletcher/58673a468c8ebbbf91b81e706063ba56.");
            }

            this.Api.Validate();
        }

        public class ServerSettings
        {
            public string Database { get; set; }

            public string IssuerUri => this.issuer_uri;

            public bool RespectXForwardedForHeaders => this.respect_x_forwarded_for_headers == false ? false : true;

            private string issuer_uri { get; set; }

            private bool? respect_x_forwarded_for_headers { get; set; }
        }

        public class ApiSettings
        {
            public string Authority { get; set; }

            public string Audience { get; set; }

            public string ClientId => this.client_id;

            public string Secret { get; set; }

            private string client_id { get; set; }

            public void Validate()
            {
                if (string.IsNullOrEmpty(this.Authority) ||
                    string.IsNullOrEmpty(this.Audience) ||
                    string.IsNullOrEmpty(this.ClientId) ||
                    string.IsNullOrEmpty(this.Secret))
                {
                    throw new InvalidOperationException(
                        "Validation of API settings failed. Please see: https://gist.github.com/cameronfletcher/58673a468c8ebbbf91b81e706063ba56.");
                }
            }
        }

        public class IdpSettings
        {
            public GoogleSettings Google { get; set; }

            public class GoogleSettings
            {
                public string ClientId => this.client_id;

                public string Secret { get; set; }

                private string client_id { get; set; }

                public bool IsValid() => !string.IsNullOrEmpty(this.ClientId) && !string.IsNullOrEmpty(this.Secret);
            }
        }

        public class MailSettings
        {
            public string Sender { get; set; }

            public string Host { get; set; }

            public int Port { get; set; }

            public bool EnableSsl => this.enable_ssl;

            public string Username { get; set; }

            public string Password { get; set; }

            private bool enable_ssl { get; set; }

            public bool IsValid() =>
                !string.IsNullOrEmpty(this.Sender) &&
                !string.IsNullOrEmpty(this.Host) &&
                !string.IsNullOrEmpty(this.Username) ? !string.IsNullOrEmpty(this.Password) : false;
        }
    }
}
