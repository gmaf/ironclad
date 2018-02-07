// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Postgresql.Mappers;
    using Ironclad.Client;
    using Marten;
    using Microsoft.AspNetCore.Mvc;
    using IdentityServerResource = IdentityServer4.Models.ApiResource;
    using IroncladResource = Ironclad.Client.Resource;
    using PostgresResource = IdentityServer4.Postgresql.Entities.ApiResource;

    [Route("api/[controller]")]
    public class ApiResourcesController : Controller
    {
        private readonly IDocumentStore store;

        public ApiResourcesController(IDocumentStore store)
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
                var totalSize = await session.Query<PostgresResource>().CountAsync();
                var apiResources = await session.Query<PostgresResource>().Skip(skip).Take(take).ToListAsync();
                var resources = apiResources.Select(
                    item =>
                    new ResourceSummaryResource
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + item.Name),
                        Name = item.Name,
                        DisplayName = item.DisplayName,
                        Enabled = item.Enabled
                    });

                return this.Ok(new ResourceSet<ResourceSummaryResource>(skip, totalSize, resources));
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            using (var session = this.store.LightweightSession())
            {
                var resource = await session.Query<PostgresResource>()
                    .SingleOrDefaultAsync(item => item.Name == name);

                if (resource == null)
                {
                    return this.NotFound();
                }

                return this.Ok(
                    new ResourceResource
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + resource.Name),
                        Name = resource.Name,
                        DisplayName = resource.DisplayName,
                        UserClaims = resource.UserClaims?.Select(item => item.Type).ToList(),
                        ApiScopes = resource.Scopes?
                            .Select(scope => new ResourceResource.Scope { Name = scope.Name, UserClaims = scope.UserClaims.Select(claim => claim.Type).ToList() })
                            .ToList(),
                        Enabled = resource.Enabled,
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IroncladResource model)
        {
            var resource = new IdentityServerResource(model.Name, model.DisplayName)
            {
                ApiSecrets = new List<Secret> { new Secret(model.ApiSecret.Sha256()) },
            };

            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresResource>().Any(client => client.Name == model.Name))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "API resource already exists" });
                }

                session.Insert(resource.ToEntity());

                await session.SaveChangesAsync();
            }

            this.Response.Headers.Add("Location", this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + resource.Name));

            return this.Ok();
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody]IroncladResource model)
        {
            using (var session = this.store.LightweightSession())
            {
                var document = await session.Query<PostgresResource>().SingleOrDefaultAsync(item => item.Name == name);
                if (document == null)
                {
                    return this.NotFound(new { Message = "API resource not found" });
                }

                // NOTE (Cameron): Because of the mapping/conversion unknowns we rely upon the Postgres integration to perform that operation which is why we do this...
                var resource = new IdentityServerResource
                {
                    UserClaims = model.UserClaims,
                    Scopes = model.ApiScopes.Select(scope => new Scope(scope.Name, scope.UserClaims)).ToList(),
                };

                // NOTE (Cameron): If the secret is updated we want to add the new secret...
                if (!string.IsNullOrEmpty(model.ApiSecret))
                {
                    resource.ApiSecrets = new List<Secret> { new Secret(model.ApiSecret.Sha256()) };
                }

                var entity = resource.ToEntity();

                // update properties (everything supported is an optional update eg. if null is passed we will not update)
                document.DisplayName = model.DisplayName ?? document.DisplayName;
                document.UserClaims = entity.UserClaims ?? document.UserClaims;
                document.Scopes = entity.Scopes ?? document.Scopes;
                document.Enabled = model.Enabled ?? document.Enabled;

                if (!string.IsNullOrEmpty(model.ApiSecret))
                {
                    document.Secrets.Add(entity.Secrets.First());
                }

                session.Update(document);

                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            using (var session = this.store.LightweightSession())
            {
                session.DeleteWhere<PostgresResource>(item => item.Name == name);
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
