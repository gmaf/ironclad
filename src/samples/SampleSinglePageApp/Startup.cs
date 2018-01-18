namespace SampleSinglePageApp
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            // enable to test w/ CSP
            //app.Use(async (ctx, next) =>
            //{
            //    ctx.Response.OnStarting(() =>
            //    {
            //        if (ctx.Response.ContentType?.StartsWith("text/html") == true)
            //        {
            //            ctx.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; connect-src http://localhost:5000 http://localhost:3721; frame-src 'self' http://localhost:5000");
            //        }
            //        return Task.CompletedTask;
            //    });

            //    await next();
            //});

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
