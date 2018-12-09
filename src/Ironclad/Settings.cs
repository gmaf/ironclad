// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

// TODO (Cameron): Refactor and remove when JSON configuration supports snake case.
#pragma warning disable IDE1006, SA1300
#pragma warning disable CA1812, CA1308

namespace Ironclad
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    internal class Settings
    {
        public ServerSettings Server { get; set; }

        public ApiSettings Api { get; set; }

        public IdpSettings Idp { get; set; }

        public MailSettings Mail { get; set; }

        public AzureSettings Azure { get; set; }

        public void Validate()
        {
            var sections = new Dictionary<string, IEnumerable<string>>();

            if (this.Server == null)
            {
                sections.Add(nameof(this.Server), new[] { "Missing section." });
            }
            else if (this.Server.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Server), this.Server.GetValidationErrors());
            }

            if (this.Api == null)
            {
                sections.Add(nameof(this.Api), new[] { "Missing section." });
            }
            else if (this.Api?.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Api), this.Api.GetValidationErrors());
            }

            if (this.Idp?.Google?.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Idp.Google), this.Idp.Google.GetValidationErrors());
            }

            if (this.Mail?.GetValidationErrors().Any() == true)
            {
                sections.Add(nameof(this.Mail), this.Mail.GetValidationErrors());
            }

            if (this.Azure?.KeyVault?.GetValidationErrors().Any() == true)
            {
                sections.Add($"{nameof(this.Azure)}:{nameof(this.Azure.KeyVault)}", this.Azure.KeyVault.GetValidationErrors());
            }

            if (sections.Any())
            {
                var builder = new StringBuilder();
                foreach (var section in sections)
                {
                    var errors = section.Value.Select(value => string.Format(CultureInfo.InvariantCulture, value, section.Key.ToLowerInvariant()));
                    builder.Append($"\r\nErrors in '{section.Key.ToLowerInvariant()}' section:\r\n - {string.Join("\r\n - ", errors)}");
                }

                // TODO (Cameron): Change link to point to somewhere sensible (when it exists).
                throw new InvalidOperationException(
                    $@"Validation of configuration settings failed.{builder.ToString()}
Please see https://gist.github.com/cameronfletcher/58673a468c8ebbbf91b81e706063ba56 for more information.");
            }
        }

        public class ServerSettings
        {
            public string Database { get; set; }

            public string IssuerUri => this.issuer_uri;

            public bool RespectXForwardedForHeaders => this.respect_x_forwarded_for_headers == false ? false : true;

            private string issuer_uri { get; set; }

            private bool? respect_x_forwarded_for_headers { get; set; }

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.Database))
                {
                    yield return $"'{{0}}:{nameof(this.Database).ToLowerInvariant()}' is null or empty.";
                }
            }
        }

        public class ApiSettings
        {
            public string Authority { get; set; }

            public string Audience { get; set; }

            public string ClientId => this.client_id;

            public string Secret { get; set; }

            private string client_id { get; set; }

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.Authority))
                {
                    yield return $"'{{0}}:{nameof(this.Authority).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.Audience))
                {
                    yield return $"'{{0}}:{nameof(this.Audience).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.ClientId))
                {
                    yield return $"'{{0}}:{nameof(this.client_id).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.Secret))
                {
                    yield return $"'{{0}}:{nameof(this.Secret).ToLowerInvariant()}' is null or empty.";
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

                public bool IsValid() => !this.GetValidationErrors().Any();

                public IEnumerable<string> GetValidationErrors()
                {
                    if (string.IsNullOrEmpty(this.ClientId))
                    {
                        yield return $"'{{0}}:{nameof(this.client_id).ToLowerInvariant()}' is null or empty.";
                    }

                    if (string.IsNullOrEmpty(this.Secret))
                    {
                        yield return $"'{{0}}:{nameof(this.Secret).ToLowerInvariant()}' is null or empty.";
                    }
                }
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

            public bool IsValid() => !this.GetValidationErrors().Any();

            public IEnumerable<string> GetValidationErrors()
            {
                if (string.IsNullOrEmpty(this.Sender))
                {
                    yield return $"'{{0}}:{nameof(this.Sender).ToLowerInvariant()}' is null or empty.";
                }

                if (string.IsNullOrEmpty(this.Host))
                {
                    yield return $"'{{0}}:{nameof(this.Host).ToLowerInvariant()}' is null or empty.";
                }

                if (!string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password))
                {
                    yield return $"'{{0}}:{nameof(this.Password).ToLowerInvariant()}' is null or empty but '{{0}}{nameof(this.Username)}' is not.";
                }
            }
        }

        public class AzureSettings
        {
            public KeyVaultSettings KeyVault => this.key_vault;

            private KeyVaultSettings key_vault { get; set; }

            public class KeyVaultSettings
            {
                public string Name { get; set; }

                public string ConnectionString { get; set; }

                public string Endpoint => $"https://{this.Name}.vault.azure.net";

                public bool IsValid() => !this.GetValidationErrors().Any();

                public IEnumerable<string> GetValidationErrors()
                {
                    if (string.IsNullOrEmpty(this.Name))
                    {
                        yield return $"'{{0}}:{nameof(this.Name).ToLowerInvariant()}' is null or empty.";
                    }
                }
            }
        }
    }
}