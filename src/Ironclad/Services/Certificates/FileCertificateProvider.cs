// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Services.Certificates
{
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class FileCertificateProvider : ICertificateProvider
    {
        private readonly string filepath;
        private readonly string password;
        private readonly ILogger<FileCertificateProvider> logger;

        public FileCertificateProvider(string filepath, string password, ILogger<FileCertificateProvider> logger)
        {
            this.filepath = filepath;
            this.password = password;
            this.logger = logger;
        }

        // TODO (Cameron): Needs to handle certs with passwords.
        public async Task<X509Certificate2> GetCertificateAsync()
        {
            this.logger.LogInformation($"Loading certificate from file path.");

            if (!File.Exists(this.filepath))
            {
                var message = $"Certificate file '{this.filepath}' not found.";

                this.logger.LogError(message);
                throw new FileNotFoundException(message);
            }

            var file = await File.ReadAllBytesAsync(this.filepath).ConfigureAwait(false);

            return new X509Certificate2(file, this.password);
        }
    }
}
