// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Services.Certificates
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Logging;

    internal class AzureKeyVaultCertificateProvider : ICertificateProvider
    {
        private readonly KeyVaultClient keyValutClient;
        private readonly string certificateId;
        private readonly ILogger<AzureKeyVaultCertificateProvider> logger;

        public AzureKeyVaultCertificateProvider(KeyVaultClient keyValutClient, string certificateId, ILogger<AzureKeyVaultCertificateProvider> logger)
        {
            this.keyValutClient = keyValutClient;
            this.certificateId = certificateId;
            this.logger = logger;
        }

        public async Task<X509Certificate2> GetCertificateAsync()
        {
            this.logger.LogInformation($"Loading certificate from Azure Key Vault.");

            var certificateSecret = await this.keyValutClient.GetSecretAsync(this.certificateId).ConfigureAwait(false);
            if (certificateSecret == null)
            {
                var message = $"Certificate with identity '{this.certificateId}' not found in Azure Key Vault.";

                this.logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var cert = new X509Certificate2(Convert.FromBase64String(certificateSecret.Value));

            return cert;
        }
    }
}
