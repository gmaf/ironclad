// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    public class ScopeHandler : AuthorizationHandler<UserAdministratorRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAdministratorRequirement requirement)
        {
            if (context.User.FindAll("scope").Any(scope => string.Equals(scope.Value, "auth_api:write", StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
