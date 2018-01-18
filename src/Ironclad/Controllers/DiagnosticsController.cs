// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace IdentityServer4.Quickstart.UI
{
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [SecurityHeaders]
    public class DiagnosticsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", this.HttpContext.Connection.LocalIpAddress.ToString() };
            if (!localAddresses.Contains(this.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return this.NotFound();
            }

            var model = new DiagnosticsModel(await this.HttpContext.AuthenticateAsync());
            return this.View(model);
        }
    }
}