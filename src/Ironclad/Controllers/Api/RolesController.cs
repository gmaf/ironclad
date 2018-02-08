namespace Ironclad.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using Ironclad.Client;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Get(int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            var totalSize = await this.roleManager.Roles.CountAsync();

            var roles = await this.roleManager.Roles.Skip(skip).Take(take).ToListAsync();

            var resources = roles.Select(
                item =>
                new RoleResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + item.Id),
                    Id = item.Id,
                    Name = item.Name
                });

            var resourceSet = new ResourceSet<RoleResource>(skip, totalSize, resources);

            return this.Ok(resourceSet);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await this.roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return this.NotFound();
            }

            return this.Ok(new RoleResource
            {
                Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + role.Id),
                Id = role.Id,
                Name = role.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Role model)
        {
            if (await this.roleManager.RoleExistsAsync(model.Name))
            {
                return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Already exist" });
            }

            await this.roleManager.CreateAsync(new IdentityRole(model.Name));

            this.Response.Headers.Add("Location", this.HttpContext.GetIdentityServerRelativeUrl("~/api/roles/" + model.Name));

            return this.Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]Role model)
        {
            model.Id = id;

            var existing = await this.roleManager.FindByIdAsync(model.Id);

            if (existing == null)
            {
                return this.NotFound();
            }

            existing.Name = model.Name ?? existing.Name;

            await this.roleManager.UpdateAsync(existing);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await this.roleManager.FindByNameAsync(id);

            if (role != null)
            {
                await this.roleManager.DeleteAsync(role);
            }

            return this.Ok();
        }

#pragma warning disable CA1034, CA1056
        public class RoleResource : Role
        {
            public string Url { get; set; }
        }
    }
}
