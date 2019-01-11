// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Authorization
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    public class RoleHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();

            foreach (var requirement in requirements)
            {
                if (requirement is SystemAdministratorRequirement)
                {
                    if (context.User.IsInRole("admin") || context.User.IsInRole("auth_admin"))
                    {
                        context.Succeed(requirement);
                    }
                }
                else if (requirement is UserAdministratorRequirement)
                {
                    if (context.User.IsInRole("admin") || context.User.IsInRole("user_admin"))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
