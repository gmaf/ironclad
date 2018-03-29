// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.WebApi
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Postgresql.Mappers;
    using Ironclad.Client;
    using Marten;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using IdentityServerResource = IdentityServer4.Models.IdentityResource;
    using IroncladResource = Ironclad.Client.IdentityResource;
    using PostgresResource = IdentityServer4.Postgresql.Entities.IdentityResource;

    [Authorize("system_admin")]
    [Route("api/[controller]")]
    public class IdentityResourcesController : Controller
    {
        private readonly IDocumentStore store;

        public IdentityResourcesController(IDocumentStore store)
        {
            this.store = store;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name, int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            using (var session = this.store.LightweightSession())
            {
                var resourceQuery = string.IsNullOrEmpty(name)
                    ? session.Query<PostgresResource>()
                    : session.Query<PostgresResource>().Where(resource => resource.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

                var totalSize = await resourceQuery.CountAsync();
                var documents = await resourceQuery.OrderBy(resource => resource.Name).Skip(skip).Take(take).ToListAsync();
                var resources = documents.Select(
                    document =>
                    new ResourceSummaryResource
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + document.Name),
                        Name = document.Name,
                        DisplayName = document.DisplayName,
                        Enabled = document.Enabled
                    });

                return this.Ok(new ResourceSet<ResourceSummaryResource>(skip, totalSize, resources));
            }
        }

        [HttpHead("{resourceName}")]
        [HttpGet("{resourceName}")]
        public async Task<IActionResult> Get(string resourceName)
        {
            using (var session = this.store.LightweightSession())
            {
                var document = await session.Query<PostgresResource>().SingleOrDefaultAsync(item => item.Name == resourceName);
                if (document == null)
                {
                    return this.NotFound(new { Message = $"Identity resource '{resourceName}' not found" });
                }

                return this.Ok(
                    new ResourceResource
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + document.Name),
                        Name = document.Name,
                        DisplayName = document.DisplayName,
                        UserClaims = document.UserClaims?.Select(item => item.Type).ToList(),
                        Enabled = document.Enabled,
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IroncladResource model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return this.BadRequest(new { Message = $"Cannot create an identity resource without a name" });
            }

            if (model.UserClaims?.Any() == false)
            {
                return this.BadRequest(new { Message = $"Cannot create an identity resource without any claims" });
            }

            var resource = new IdentityServerResource(model.Name, model.DisplayName, model.UserClaims);

            // optional properties
            resource.Enabled = model.Enabled ?? resource.Enabled;

            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresResource>().Any(item => item.Name == model.Name))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Identity resource already exists" });
                }

                session.Insert(resource.ToEntity());

                await session.SaveChangesAsync();
            }

            return this.Created(new Uri(this.HttpContext.GetIdentityServerRelativeUrl("~/api/identityresources/" + model.Name)), null);
        }

        [HttpPut("{resourceName}")]
        public async Task<IActionResult> Put(string resourceName, [FromBody]IroncladResource model)
        {
            if (model.UserClaims?.Any() == false)
            {
                return this.BadRequest(new { Message = $"Cannot update an identity resource without any claims" });
            }

            using (var session = this.store.LightweightSession())
            {
                var document = await session.Query<PostgresResource>().SingleOrDefaultAsync(item => item.Name == resourceName);
                if (document == null)
                {
                    return this.NotFound(new { Message = $"Identity resource '{resourceName}' not found" });
                }

                // NOTE (Cameron): Because of the mapping/conversion unknowns we rely upon the Postgres integration to perform that operation which is why we do this...
                var resource = new IdentityServerResource
                {
                    UserClaims = model.UserClaims,
                };

                var entity = resource.ToEntity();

                // update properties (everything supported is an optional update eg. if null is passed we will not update)
                document.DisplayName = model.DisplayName ?? document.DisplayName;
                document.UserClaims = entity.UserClaims ?? document.UserClaims;
                document.Enabled = model.Enabled ?? document.Enabled;

                session.Update(document);

                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

        [HttpDelete("{resourceName}")]
        public async Task<IActionResult> Delete(string resourceName)
        {
            using (var session = this.store.LightweightSession())
            {
                session.DeleteWhere<PostgresResource>(document => document.Name == resourceName);
                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

#pragma warning disable CA1034, CA1056
        public class ResourceResource : IroncladResource
        {
            public string Url { get; set; }
        }

        private class ResourceSummaryResource : ResourceSummary
        {
            public string Url { get; set; }
        }
    }
}
