// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Routing;

namespace Ironclad
{
    using IdentityModel.Client;
    using IdentityServer4.AccessTokenValidation;
    using IdentityServer4.Postgresql.Extensions;
    using Ironclad.Application;
    using Ironclad.Authorization;
    using Ironclad.Data;
    using Ironclad.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        private readonly ILogger<Startup> logger;
        private readonly IConfiguration configuration;

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            this.logger = logger;
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

            // TODO (Cameron): This is a bit messy. I think ultimately this should be configurable inside the application itself.
            var mailUsername = this.configuration.GetValue<string>("Mail:Username");
            if (string.IsNullOrEmpty(mailUsername))
            {
                this.logger.LogWarning("No credentials specified for SMTP. Email will be disabled.");
                services.AddSingleton<IEmailSender>(new NullEmailSender());
            }
            else
            {
                services.AddSingleton<IEmailSender>(
                    new EmailSender(
                        this.configuration.GetValue<string>("Mail:Sender"),
                        this.configuration.GetValue<string>("Mail:Host"),
                        this.configuration.GetValue<int>("Mail:Port"),
                        this.configuration.GetValue<bool>("Mail:EnableSSL"),
                        mailUsername,
                        this.configuration.GetValue<string>("Mail:Password")));
            }

            services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
            services.AddSingleton<IAuthorizationHandler, RoleHandler>();

            services.AddMvc()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });

            services.AddIdentityServer(options => options.IssuerUri = this.configuration.GetValue<string>("issuerUri"))
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(this.configuration.GetConnectionString("Ironclad"))
                .AddOperationalStore()
                .AddAppAuthRedirectUriValidator()
                .AddAspNetIdentity<ApplicationUser>();

            services.AddMultiTenant()
                .WithInMemoryStore(configuration.GetSection("Finbuckle:MultiTenant:InMemoryStore"))
                .WithStaticStrategy("ironclad")
                .WithRemoteAuthentication()
                .WithPerTenantOptions<Microsoft.AspNetCore.Authentication.AuthenticationOptions>((options, tenantContext) =>
                {
                    // Allow each tenant to have a different default challenge scheme.
                    if (tenantContext.Items.TryGetValue("ChallengeScheme", out object challengeScheme))
                    {
                        options.DefaultChallengeScheme = (string)challengeScheme;
                    }
                }).
                WithPerTenantOptions<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions>((options, tenantContext) =>
                {
                    // Set a unique cookie name for this tenant.
                    options.Cookie.Name = tenantContext.Id + "-cookie";

                    // Note the paths set take our routing strategy into account.
                    options.LoginPath = "/" + tenantContext.Identifier + "/Home/Login";
                    options.Cookie.Path = "/" + tenantContext.Identifier;
                });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(
                    "OpenIdConnect",
                    "Ironclad Test Tennant",
                    options =>
                    {
                        options.ClientId = "lykke-oidc";
                        options.Authority = "https://auth-test.lykkecloud.com";
                    })
                .AddGoogle(
                    options =>
                    {
                        options.ClientId = this.configuration.GetValue<string>("Google-ClientId");
                        options.ClientSecret = this.configuration.GetValue<string>("Google-Secret");
                    })
                .AddIdentityServerAuthentication(
                    "token",
                    options =>
                    {
                        options.Authority = this.configuration.GetValue<string>("authority");
                        options.Audience = $"{this.configuration.GetValue<string>("issuerUri")}/resources";
                        options.RequireHttpsMetadata = false;
                    },
                    options =>
                    {
                        options.Authority = this.configuration.GetValue<string>("authority");
                        options.ClientId = "auth_api";
                        options.ClientSecret = this.configuration.GetValue<string>("Introspection-Secret");
                        options.DiscoveryPolicy = new DiscoveryPolicy { ValidateIssuerName = false };
                    });

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("auth_admin", policy => policy.AddAuthenticationSchemes("token").Requirements.Add(new SystemAdministratorRequirement()));
                    options.AddPolicy("user_admin", policy => policy.AddAuthenticationSchemes("token").Requirements.Add(new UserAdministratorRequirement()));
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            if (this.configuration.GetValue<bool>("respectXForwardedForHeaders"))
            {
                var options = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto };
                app.UseForwardedHeaders(options);
            }

            app.UseStaticFiles();

            app.UseMultiTenant();

            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
            app.InitializeDatabase().SeedDatabase(this.configuration);
        }
    }
}
