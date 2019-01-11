// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Services.Certificates
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class LocalMachineStoreCertificateProvider : ICertificateProvider
    {
        private readonly string thumbprint;
        private readonly ILogger<LocalMachineStoreCertificateProvider> logger;

        public LocalMachineStoreCertificateProvider(string thumbprint, ILogger<LocalMachineStoreCertificateProvider> logger)
        {
            this.thumbprint = thumbprint;
            this.logger = logger;
        }

        public Task<X509Certificate2> GetCertificateAsync()
        {
            this.logger.LogInformation($"Loading certificate from local store.");

            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);

                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, this.thumbprint, false);
                if (certificates.Count == 0)
                {
                    var message = $"Certificate '{this.thumbprint}' not found in local store.";

                    this.logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                return Task.FromResult(certificates[0]);
            }
        }
    }
}
