// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Sdk
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result as ViewResult;
            if (result == null)
            {
                return;
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
            {
                context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            }

            // https://scotthelme.co.uk/hsts-the-missing-link-in-tls/
            var hsts = "Strict-Transport-Security: max-age=31536000; includeSubDomains";
            if (!context.HttpContext.Response.Headers.ContainsKey("Strict-Transport-Security"))
            {
                context.HttpContext.Response.Headers.Add("Strict-Transport-Security", hsts);
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
            var appInsightsScript = "https://az416426.vo.msecnd.net/scripts/a/ai.0.js";
            var csp = $"script-src 'self' 'sha256-SjXRkVC/0M0+WLq2GU4E8JdbZ/ZNgspoHSzWQaMhG7E=' {appInsightsScript}; img-src 'self'; style-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
            //// also consider adding upgrade-insecure-requests once you have HTTPS in place for production
            ////csp += "upgrade-insecure-requests;";
            //// also an example if you need client images to be displayed from twitter
            //// csp += "img-src 'self' https://pbs.twimg.com;";

            // once for standards compliant browsers
            if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
            }

            // and once again for IE
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
            {
                context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
            var referrer_policy = "no-referrer";
            if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
            {
                context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
            }

            // https://scotthelme.co.uk/hardening-your-http-response-headers/#x-xss-protection
            if (!context.HttpContext.Response.Headers.ContainsKey("X-XSS-Protection"))
            {
                context.HttpContext.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            }

            // https://scotthelme.co.uk/a-new-security-header-feature-policy/
            var features = "geolocation 'none'; microphone 'none'; camera 'none'";
            if (!context.HttpContext.Response.Headers.ContainsKey("Feature-Policy"))
            {
                context.HttpContext.Response.Headers.Add("Feature-Policy", features);
            }
        }
    }
}