// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    // NOTE (Cameron): This is required for Entity Framework migrations as the signature of BuildWebHost has been modified.
    // LINK (Cameron): https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables()
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(configuration.GetConnectionString("Ironclad"))
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
