// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using IdentityServer4.Postgresql.Extensions;
    using Ironclad.Application;
    using Ironclad.Data;
    using Ironclad.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetType().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(this.configuration.GetConnectionString("Ironclad")));

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Tokens.ChangePhoneNumberTokenProvider = "Phone";

                    // LINK (Cameron): https://pages.nist.gov/800-63-3/
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddIdentityServer(options => options.PublicOrigin = this.configuration.GetValue<string>("PUBLIC_ORIGIN"))
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(this.configuration.GetConnectionString("Ironclad"))
                .AddOperationalStore()
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication()
                .AddGoogle(
                    options =>
                    {
                        options.ClientId = this.configuration.GetValue<string>("Google-ClientId");
                        options.ClientSecret = this.configuration.GetValue<string>("Google-Secret");
                    });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
            app.InitializeDatabase().SeedDatabase();
        }
    }
}
