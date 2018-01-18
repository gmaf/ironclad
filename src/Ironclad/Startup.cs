// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private static readonly string VersionJson = GetVersionJson();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients(Config.GetInMemoryClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddTestUsers(IdentityServer4.Quickstart.UI.TestUsers.Users)
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            app.UseIdentityServer();

            // TODO (Cameron): Introduce common status code handling (somehow).
            app.Run(async (context) =>
            {
                if (context.Request.Path == "/")
                {
                    await context.Response.WriteAsync(VersionJson).ConfigureAwait(false);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
        }

        private static string GetVersionJson()
        {
            var assembly = typeof(Program).Assembly;
            var title = assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title);
            var version = assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion);

            return $@"{{""title"":""{title}"",""version"":""{version}"",""os"":""{System.Runtime.InteropServices.RuntimeInformation.OSDescription}""}}";
        }
    }
}
