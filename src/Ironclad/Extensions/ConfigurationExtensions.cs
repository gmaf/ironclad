// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Extensions
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;

    internal static class ConfigurationExtensions
    {
        internal static IConfigurationBuilder AddAzureKeyVaultSecrets(this IConfigurationBuilder builder)
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<Startup>().AddEnvironmentVariables().Build();

            var azureKeyVaultSection = configuration.GetSection("AzureVaultSettings");
            var kv = azureKeyVaultSection.Get<Settings.AzureKeyVaultSettings>();

            if (!string.IsNullOrEmpty(kv.VaultName))
            {
                // Note (Pawel) If user provided KV name without identity secrets, let him use user-assigned managed identity
                var tokenProviderConnString = !string.IsNullOrEmpty(kv.IdentityApplicationId) ?
                    $"RunAs=App;AppId={kv.IdentityApplicationId};TenantId={kv.IdentityTenantId};AppKey={kv.IdentityClientSecret}"
                    : null;

                var tokenProvider = new AzureServiceTokenProvider(tokenProviderConnString);
                var keyVaultClient = new KeyVaultClient(
                          new KeyVaultClient.AuthenticationCallback(
                              tokenProvider.KeyVaultTokenCallback));

                return builder.AddAzureKeyVault(
                           kv.Endpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
            }

            return builder;
        }
    }
}
