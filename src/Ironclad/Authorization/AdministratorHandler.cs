// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Authorization
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    public class AdministratorHandler : AuthorizationHandler<AdministratorRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdministratorRequirement requirement)
        {
            var adminRole = $"{requirement.Type}_admin";

            if (context.User.IsInRole("admin") || context.User.IsInRole(adminRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
