namespace SampleWebApi
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;

    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claims = new List<Claim>
            {
            };

            var identity = new ClaimsIdentity(claims, principal.Identity.AuthenticationType, "name", "role");

            return Task.FromResult(new ClaimsPrincipal(identity));
        }
    }
}
