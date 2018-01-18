namespace Ironclad.Configurations
{
    using System.Linq;
    using IdentityServer4.Postgresql.Mappers;
    using Marten;
    using Microsoft.AspNetCore.Builder;
    using Serilog;

    public class PostgresDatabaseSeeder : IDatabaseSeeder
    {
        private readonly PostgresConfig config;

        public PostgresDatabaseSeeder(PostgresConfig config)
        {
            this.config = config;
        }

        public void Seed(IApplicationBuilder app)
        {
            Log.Information($"Seeding postgres database: {this.config.Database}");

            var store = DocumentStore.For(this.config.ToString());

            Log.Information($"Clearing all data from database: {this.config.Database}");

            store.Advanced.Clean.CompletelyRemoveAll();

            using (var session = store.LightweightSession())
            {
                if (!session.Query<IdentityServer4.Postgresql.Entities.Client>().Any())
                {
                    Log.Information("Adding clients");

                    session.StoreObjects(Config.GetInMemoryClients().Select(c => c.ToEntity()));
                }

                if (!session.Query<IdentityServer4.Postgresql.Entities.ApiResource>().Any())
                {
                    Log.Information("Adding ApiResources");

                    session.StoreObjects(Config.GetApiResources().Select(r => r.ToEntity()));
                }

                if (!session.Query<IdentityServer4.Postgresql.Entities.IdentityResource>().Any())
                {
                    Log.Information("Adding IdentityResources");

                    session.StoreObjects(Config.GetIdentityResources().Select(r => r.ToEntity()));
                }

                session.SaveChanges();

                Log.Information($"Saving all changes to database: {this.config.Database}");
            }

            Log.Information($"Seed Completed");
        }
    }
}
