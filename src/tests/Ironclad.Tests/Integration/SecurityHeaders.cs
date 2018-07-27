// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Feature
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Ironclad.Client;
    using Ironclad.Tests.Sdk;
    using Xunit;

    public class SecurityHeaders : IntegrationTest
    {
        public SecurityHeaders(AuthenticationFixture securityFixture, IroncladFixture ironcladFixture, PostgresFixture postgresFixture)
            : base(securityFixture, ironcladFixture, postgresFixture)
        {
        }

        [Fact]
        public async Task ShouldConformToSecurityHeadersIO()
        {
            /* https://securityheaders.io */

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, this.Authority))
            using (var response = await client.SendAsync(request).ConfigureAwait(false))
            {
                Assert.Equal(new[] { "nosniff" },                               response.Headers.GetValues("X-Content-Type-Options"));
                Assert.Equal(new[] { "SAMEORIGIN" },                            response.Headers.GetValues("X-Frame-Options"));
                Assert.Equal(new[] { "no-referrer" },                           response.Headers.GetValues("Referrer-Policy"));
                Assert.Equal(new[] { "max-age=31536000" },                      response.Headers.GetValues("Strict-Transport-Security"));
                Assert.Equal(new[] { "1; mode=block" },                         response.Headers.GetValues("X-XSS-Protection"));

                Assert.Equal(
                    new[]
                    {
                    "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';"
                    },                                                          response.Headers.GetValues("Content-Security-Policy"));
                Assert.Equal(
                    new[]
                    {
                        "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';"
                    },                                                          response.Headers.GetValues("X-Content-Security-Policy"));
                Assert.Equal(
                   new[]
                   {
                        "geolocation 'none'; midi 'none'; notifications 'self'; push 'self'; microphone 'none'; camera 'none'; magnetometer 'none'; gyroscope 'none'; speaker 'none'; vibrate 'none'; payment 'none'"
                   },                                                           response.Headers.GetValues("Feature-Policy"));
            }
        }
    }
}
