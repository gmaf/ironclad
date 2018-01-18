namespace SampleWebApi
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    // decorating this class with the Authorize attribute will require the user to be authenticated before any method gets invoked
    [Authorize]
    public class SampleController : ControllerBase
    {
        [HttpGet("api")]
        public IActionResult Success()
        {
            // this is an example of pulling out the 'user' from the context of the request
            // the 'user' *will* exist as authentication has taken place; however the 'user' may be a client and not an end-user based on the permission you have set
            var principal = this.User;

            return this.Ok(new { message = $"Hello {principal.Identity.Name ?? principal.Claims.SingleOrDefault(c => c.Type == "client_id")?.Value ?? "{{unknown}}"}" });
        }

        // if the user is authenticated but is trying to do something that they do not have permission to do then we should forbid them
        [HttpGet("forbidden")]
        public IActionResult Failure() => this.Forbid();
    }
}
