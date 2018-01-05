﻿// <copyright file="Program.cs" company="Lykke">
// Copyright (c) Lykke. All rights reserved.
// </copyright>

namespace Ironclad
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public static class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
    }
}
