// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Application
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            ////if (!string.IsNullOrWhiteSpace(user.DisplayName))
            ////{
            ////    identity.AddClaims(new[] { new Claim("displayname", user.DisplayName) });
            ////}

            ////if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            ////{
            ////    ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("avatar", user.AvatarUrl) });
            ////}

            return principal;
        }
    }
}