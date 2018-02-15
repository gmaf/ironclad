// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA2234, CA1054

namespace Ironclad.Tests.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using IdentityModel.Client;

    public class BrowseR : HttpClient
    {
        private readonly string authority;
        private readonly BrowserHandler handler;

        public BrowseR(string authority, BrowserHandler handler)
            : base(handler)
        {
            this.authority = authority;
            this.handler = handler;
        }

        public async Task<AuthorizeResponse> RequestAuthorizationEndpointAsync(
            string clientId,
            string responseType,
            string scope = null,
            string redirectUri = null,
            string state = null,
            string nonce = null,
            string loginHint = null,
            string acrValues = null,
            string responseMode = null,
            string codeChallenge = null,
            string codeChallengeMethod = null,
            object extra = null)
        {
            var originalAllowAutoRedirect = this.handler.AllowAutoRedirect;
            var originalStopRedirectingAfter = this.handler.StopRedirectingAfter;

            // here we want to auto-redirect to the login page...
            this.handler.AllowAutoRedirect = true;
            this.handler.StopRedirectingAfter = 20;

            var authorizeUrl = new RequestUrl(this.authority + "/connect/authorize").CreateAuthorizeUrl(clientId, responseType, scope, redirectUri, "state", "nonce");
            var loginResult = await this.GetAsync(authorizeUrl).ConfigureAwait(false);
            loginResult.StatusCode.Should().Be(HttpStatusCode.OK);

            // grab the necessary items for the POST...
            var loginPageHtml = await loginResult.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actionStartIndex = loginPageHtml.IndexOf("<form method=\"post\" action=\"", 0, StringComparison.OrdinalIgnoreCase) + 28;
            var action = loginPageHtml.Substring(actionStartIndex, loginPageHtml.IndexOf("\"", actionStartIndex, StringComparison.OrdinalIgnoreCase) - actionStartIndex);
            var tokenStartIndex = loginPageHtml.IndexOf("<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"", 0, StringComparison.OrdinalIgnoreCase) + 62;
            var token = loginPageHtml.Substring(tokenStartIndex, loginPageHtml.IndexOf("\"", tokenStartIndex, StringComparison.OrdinalIgnoreCase) - tokenStartIndex);

            var form = new Dictionary<string, string>
            {
                { "Username", "admin" },
                { "Password", "password" },
                { "__RequestVerificationToken", token },
                { "RememberMe", "false" },
            };

            // here we *do not* want to auto-redirect to the client (2nd redirect)...
            this.handler.StopRedirectingAfter = 1;

            var redirect = default(string);
            using (var content = new FormUrlEncodedContent(form))
            {
                var authorizeResult = await this.PostAsync(this.authority + action, content).ConfigureAwait(false);
                authorizeResult.StatusCode.Should().Be(HttpStatusCode.Found);

                redirect = authorizeResult.Headers.Location.ToString();
            }

            // reset the behavior of the browseR
            this.handler.AllowAutoRedirect = originalAllowAutoRedirect;
            this.handler.StopRedirectingAfter = originalStopRedirectingAfter;

            if (redirect.StartsWith(this.authority + "/home/error", StringComparison.OrdinalIgnoreCase))
            {
                // request error page in pipeline so we can get error info
                await this.GetAsync(redirect).ConfigureAwait(false);

                // no redirect to client
                return null;
            }

            return new AuthorizeResponse(redirect);
        }
    }
}