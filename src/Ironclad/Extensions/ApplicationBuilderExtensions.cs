// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Linq;
    using IdentityServer4.Postgresql.Mappers;
    using Ironclad.Application;
    using Ironclad.Configuration;
    using Ironclad.Data;
    using Marten;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    // TODO (Cameron): Contents of this should really be moved out into a class under the Data namespace.
    public static class ApplicationBuilderExtensions
    {
        private const string DefaultAdminUserId = "e4744f59155040599fb057d62e84c079";

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

        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var user = userManager.FindByIdAsync(DefaultAdminUserId).Result;
                if (user != null)
                {
                    return app;
                }

                Log.Information("Configuring system for first use...");

                user = userManager.FindByNameAsync("admin").Result;
                if (user != null)
                {
                    userManager.DeleteAsync(user).Wait();
                }

                user = new ApplicationUser { Id = DefaultAdminUserId, UserName = "admin" };

                roleManager.CreateAsync(new IdentityRole("admin")).Wait();
                roleManager.CreateAsync(new IdentityRole("auth_admin")).Wait();
                roleManager.CreateAsync(new IdentityRole("user_admin")).Wait();

                userManager.CreateAsync(user = new ApplicationUser { Id = DefaultAdminUserId, UserName = "admin" }, "password").Wait();
                userManager.AddToRoleAsync(user, "admin").Wait();

                // NOTE (Cameron): Set up default clients in Postgres.
                var store = serviceScope.ServiceProvider.GetRequiredService<IDocumentStore>();

                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    if (!session.Query<IdentityServer4.Postgresql.Entities.Client>().Any())
                    {
                        Log.Information("Adding default clients...");
                        session.StoreObjects(Config.GetDefaultClients().Select(c => c.ToEntity()));
                    }

                    if (!session.Query<IdentityServer4.Postgresql.Entities.ApiResource>().Any())
                    {
                        Log.Information("Adding default API resources...");
                        session.StoreObjects(Config.GetDefaultApiResources(configuration).Select(r => r.ToEntity()));
                    }

                    if (!session.Query<IdentityServer4.Postgresql.Entities.IdentityResource>().Any())
                    {
                        Log.Information("Adding default identity resources...");
                        session.StoreObjects(Config.GetDefaultIdentityResources().Select(r => r.ToEntity()));
                    }

                    session.SaveChanges();
                }
            }

            return app;
        }
    }
}