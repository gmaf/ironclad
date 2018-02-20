// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.WebApi
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using Ironclad.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize("user_admin")]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name, int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            var roleQuery = string.IsNullOrEmpty(name)
                ? this.roleManager.Roles
                : this.roleManager.Roles.Where(role => role.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

            var totalSize = await roleQuery.CountAsync();
            var roles = await roleQuery.OrderBy(role => role.Name).Skip(skip).Take(take).ToListAsync();
            var resources = roles.Select(
                role =>
                new RoleResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + role.Name),
                    Name = role.Name,
                });

            var resourceSet = new ResourceSet<RoleResource>(skip, totalSize, resources);

            return this.Ok(resourceSet);
        }

        [HttpHead("{roleName}")]
        [HttpGet("{roleName}")]
        public async Task<IActionResult> Get(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return this.NotFound(new { Message = $"Role '{roleName}' not found" });
            }

            return this.Ok(
                new RoleResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + role.Name),
                    Name = role.Name
                });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Role model)
        {
            if (await this.roleManager.RoleExistsAsync(model.Name))
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Role already exists" });
            }

            var result = await this.roleManager.CreateAsync(new IdentityRole(model.Name));
            if (!result.Succeeded)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = result.ToString() });
            }

            return this.Created(new Uri(this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + model.Name)), null);
        }

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> Delete(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                await this.roleManager.DeleteAsync(role);
            }

            return this.Ok();
        }

        private class RoleResource : Role
        {
            public string Url { get; set; }
        }
    }
}
