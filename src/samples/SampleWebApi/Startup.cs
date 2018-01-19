﻿namespace SampleWebApi
{
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // here we add MvcCore because this is a web API and are going to opt-in to additional features
            services.AddMvcCore()
                .AddJsonFormatters() // opt-in to use JSON formatters
                .AddAuthorization(); // opt-in to use authorization

            // here we configure the authentication handlers to accept both JWTs and reference tokens as access tokens for the API
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.Authority = "http://localhost:5005/";   // the IdentityServer root URL (used for discovery)
                        options.ApiName = "sample_api";                 // the name of the web API (used to scope access)
                        options.ApiSecret = "secret";                   // the secret associated with the introspection endpoint for reference token validation and claims

                        // not to be set in production!
                        options.RequireHttpsMetadata = false;           // allow non-HTTPS for testing only
                    });

            // https://stackoverflow.com/questions/37916051/claims-in-jwt-vs-claims-transformation-in-resource
            ////services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

            // we have to enable CORS for any clients that come via the browser; this will allow calls to the API from the Singe Page Application
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("spa", policy =>
                {
                    policy.WithOrigins("http://localhost:5008")         // the origin specified is for the Single Page Application
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseCors("spa");

            // we should always place the authentication middleware call above the middleware that we want to secure; typically, MVC
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
