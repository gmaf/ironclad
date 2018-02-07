// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Postgresql.Mappers;
    using Ironclad.Client;
    using Marten;
    using Microsoft.AspNetCore.Mvc;
    using PostgresIdentityResource = IdentityServer4.Postgresql.Entities.IdentityResource;

    [Route("api/[controller]")]
    public class IdentityResourcesController : Controller
    {
        private readonly IDocumentStore store;

        public IdentityResourcesController(IDocumentStore store)
        {
            this.store = store;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            using (var session = this.store.LightweightSession())
            {
                var totalSize = await session.Query<PostgresIdentityResource>().CountAsync();
                var identityResources = await session.Query<PostgresIdentityResource>().Skip(skip).Take(take).ToListAsync();

                var resources = identityResources.Select(item =>
                new IdentityResourceSummaryResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + item.Name),
                    Name = item.Name,
                    DisplayName = item.DisplayName,
                    Enabled = item.Enabled
                });

                var resourceSet = new ResourceSet<IdentityResourceSummaryResource>(skip, totalSize, resources);

                return this.Ok(resourceSet);

                /*
                 * Name
                 * DisplayName
                 * UserClaims
                 * Required
                 * Emphasize
                 * ShowInDiscoverDocument
                 * Description
                 * Enabled
                 */
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            using (var session = this.store.LightweightSession())
            {
                var totalSize = await session.Query<PostgresIdentityResource>().CountAsync();
                var identityResource = await session.Query<PostgresIdentityResource>()
                    .SingleOrDefaultAsync(item => item.Name == name);

                if (identityResource == null)
                {
                    return this.NotFound();
                }

                return this.Ok(
                    new
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + identityResource.Name),
                        identityResource.Name,
                        identityResource.DisplayName,
                        UserClaims = identityResource.UserClaims?.Select(item => item.Type).ToArray(),
                        identityResource.Description,
                        identityResource.Enabled
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IdentityResource model)
        {
            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresIdentityResource>().Any(client => client.Name == model.Name))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Identity resource already exists" });
                }

                session.Insert(model.ToEntity());

                await session.SaveChangesAsync();
            }

            this.Response.Headers.Add("Location", this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + model.Name));

            return this.Ok();
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody]IdentityResource model)
        {
            model.Name = name;

            using (var session = this.store.LightweightSession())
            {
                var identityResource = await session.Query<PostgresIdentityResource>().SingleOrDefaultAsync(item => item.Name == model.Name);
                if (identityResource == null)
                {
                    return this.NotFound(new { Message = "Identity resource not found" });
                }

                var entity = model.ToEntity();
                entity.Id = identityResource.Id;

                session.Store(entity);

                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            using (var session = this.store.LightweightSession())
            {
                session.DeleteWhere<PostgresIdentityResource>(item => item.Name == name);
                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

#pragma warning disable CA1034
        private class IdentityResourceSummaryResource : ResourceSummary
        {
            public string Url { get; set; }
        }
    }
}
