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
        private readonly string connectionString;
        private readonly string certificateId;
        private readonly ILogger<AzureKeyVaultCertificateProvider> logger;

        public AzureKeyVaultCertificateProvider(string connectionString, string certificateId, ILogger<AzureKeyVaultCertificateProvider> logger)
        {
            this.connectionString = connectionString;
            this.certificateId = certificateId;
            this.logger = logger;
        }

        public async Task<X509Certificate2> GetCertificateAsync()
        {
            this.logger.LogInformation($"Loading certificate from Azure Key Vault.");

            var tokenProvider = new AzureServiceTokenProvider(this.connectionString);

            using (var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback)))
            {
                var certificate = await keyVaultClient.GetCertificateAsync(this.certificateId).ConfigureAwait(false);
                if (certificate == null)
                {
                    var message = $"Certificate with identity '{this.certificateId}' not found in Azure Key Vault.";

                    this.logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                return new X509Certificate2(certificate.Cer);
            }
        }
    }
}
