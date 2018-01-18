namespace Ironclad.Configurations
{
    using System.Linq;
    using IdentityServer4.Postgresql.Mappers;
    using Marten;
    using Serilog;

    public class PostgresDatabaseSeeder : IDatabaseSeeder
    {
        private readonly IDocumentStore context;

        public PostgresDatabaseSeeder(IDocumentStore context)
        {
            this.context = context;
        }

        public void Seed()
        {
            Log.Information($"Start Seeding postgres database");

            Log.Information($"Clearing all data from database");

            this.context.Advanced.Clean.CompletelyRemoveAll();

            using (var session = this.context.LightweightSession())
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

                Log.Information($"Saving all changes to database");
            }

            Log.Information($"Seed Completed");
        }
    }
}
