namespace SampleWebApi
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class SampleController : ControllerBase
    {
        [HttpGet("api")]
        public IActionResult Success()
        {
            var principal = this.User;

            return this.Ok(new { message = "Secure data!" });
        }

        [HttpGet("forbidden")]
        public IActionResult Failure() => this.Forbid();
    }
}
