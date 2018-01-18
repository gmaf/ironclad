// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace IdentityServer4.Quickstart.UI
{
    using System.Threading.Tasks;
    using IdentityServer4.Services;
    using Ironclad.Models;
    using Microsoft.AspNetCore.Mvc;

    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            this.interaction = interaction;
        }

        public IActionResult Index() => this.View();

        public async Task<IActionResult> Error(string errorId)
        {
            var model = new ErrorModel();

            var message = await this.interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                model.Error = message;
            }

            return this.View("Error", model);
        }
    }
}