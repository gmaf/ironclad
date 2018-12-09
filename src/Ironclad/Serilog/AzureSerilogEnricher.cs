// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Serilog
{
    using System;
    using Serilog.Core;
    using Serilog.Events;

    public class AzureSerilogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "localhost";
            var property = propertyFactory.CreateProperty("AzureWebsiteInstanceId", instanceId.Substring(0, Math.Min(9, instanceId.Length)));
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}