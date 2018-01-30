// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Linq;
    using IdentityServer4.Postgresql.Mappers;
    using Ironclad.Data;
    using Marten;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // NOTE (Cameron): Set up ASP.NET Core Identity using Entity Framework (with Postgres).
                var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationDbContext.Database.Migrate();
            }

            return app;
        }

        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // NOTE (Cameron): Set up default clients in Postgres.
                var store = serviceScope.ServiceProvider.GetRequiredService<IDocumentStore>();
                using (var session = store.LightweightSession())
                {
                    if (!session.Query<IdentityServer4.Postgresql.Entities.Client>().Any())
                    {
                        Log.Information("Adding initial clients...");
                        session.StoreObjects(Config.GetInMemoryClients().Select(c => c.ToEntity()));
                    }

                    if (!session.Query<IdentityServer4.Postgresql.Entities.ApiResource>().Any())
                    {
                        Log.Information("Adding initial API resources...");
                        session.StoreObjects(Config.GetApiResources().Select(r => r.ToEntity()));
                    }

                    if (!session.Query<IdentityServer4.Postgresql.Entities.IdentityResource>().Any())
                    {
                        Log.Information("Adding initial identity resources...");
                        session.StoreObjects(Config.GetIdentityResources().Select(r => r.ToEntity()));
                    }

                    session.SaveChanges();
                }
            }

            return app;
        }
    }
}