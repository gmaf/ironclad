namespace Ironclad.Configurations
{
    using System.Linq;
    using IdentityServer4.Postgresql.Mappers;
    using Marten;
    using Microsoft.AspNetCore.Builder;

    public class PostgresDatabaseSeeder : IDatabaseSeeder
    {
        private readonly PostgresConfig config;

        public PostgresDatabaseSeeder(PostgresConfig config)
        {
            this.config = config;
        }

        public void Seed(IApplicationBuilder app)
        {
            var store = DocumentStore.For(this.config.ToString());
            store.Advanced.Clean.CompletelyRemoveAll();
            using (var session = store.LightweightSession())
            {
                if (!session.Query<IdentityServer4.Postgresql.Entities.Client>().Any())
                {
                    session.StoreObjects(Config.GetInMemoryClients().Select(c => c.ToEntity()));
                }

                if (!session.Query<IdentityServer4.Postgresql.Entities.ApiResource>().Any())
                {
                    session.StoreObjects(Config.GetApiResources().Select(r => r.ToEntity()));
                }

                if (!session.Query<IdentityServer4.Postgresql.Entities.IdentityResource>().Any())
                {
                    session.StoreObjects(Config.GetIdentityResources().Select(r => r.ToEntity()));
                }

                session.SaveChanges();
            }
        }
    }
}
