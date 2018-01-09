// <copyright file="Program.cs" company="Lykke">
// Copyright (c) Lykke. All rights reserved.
// </copyright>

namespace Ironclad
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public static class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args)
        {
            // HACK (Cameron): Currently, there is no nice way to get a handle on IHostingEnvironment inside of Main() so we work around this...
            // LINK (Cameron): https://github.com/aspnet/KestrelHttpServer/issues/1334
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
