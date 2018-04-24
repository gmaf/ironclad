// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Upgrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Postgresql.Entities;
    using IdentityServer4.Stores;
    using Marten;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class BackwardsCompatiabilityManager
    {
        private readonly IServiceScope serviceScope;

        public BackwardsCompatiabilityManager(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        // NOTE (Cameron): This is very targeted towards an issue that we have with data and will probably break stuff.
        public async Task FixInvalidScopesFollowingIdentityServerUpgrade()
        {
            var resourceStore = this.serviceScope.ServiceProvider.GetService<IResourceStore>();
            var invalidScopes = GetInvalidScopes(resourceStore);
            if (!invalidScopes.Any())
            {
                return;
            }

            var logger = this.serviceScope.ServiceProvider.GetService<ILogger<BackwardsCompatiabilityManager>>();
            logger.LogWarning($"Attempting to remove duplicate scopes '{string.Join(", ", invalidScopes)}'...");

            var store = this.serviceScope.ServiceProvider.GetService<IDocumentStore>();
            var allResources = await resourceStore.GetAllResourcesAsync();
            foreach (var apiResource in allResources.ApiResources)
            {
                using (var session = store.LightweightSession())
                {
                    var document = await session.Query<ApiResource>().SingleOrDefaultAsync(item => item.Name == apiResource.Name);
                    if (document == null)
                    {
                        continue;
                    }

                    foreach (var apiScope in document.Scopes.ToList())
                    {
                        if (invalidScopes.Except(new[] { apiResource.Name }).Contains(apiScope.Name))
                        {
                            logger.LogWarning($"Removing scope '{apiScope.Name}' from API resource '{apiResource.Name}' as it is duplicated elsewhere.");
                            document.Scopes.Remove(apiScope);
                        }
                    }

                    session.Update(document);

                    await session.SaveChangesAsync();
                }
            }
        }

        private static IEnumerable<string> GetInvalidScopes(IResourceStore resourceStore)
        {
            try
            {
                IResourceStoreExtensions.GetAllEnabledResourcesAsync(resourceStore).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // HACK (Cameron): This is fairly brittle but not sure about an alternative at the moment.
                return ex.Message.Substring(112).Split(", ", StringSplitOptions.RemoveEmptyEntries);
            }

            return Enumerable.Empty<string>();
        }
    }
}
