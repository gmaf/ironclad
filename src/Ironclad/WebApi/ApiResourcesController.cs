// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.WebApi
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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using IdentityServerResource = IdentityServer4.Models.ApiResource;
    using IroncladResource = Ironclad.Client.ApiResource;
    using PostgresResource = IdentityServer4.Postgresql.Entities.ApiResource;

    [Authorize("auth_admin")]
    [Route("api/[controller]")]
    public class ApiResourcesController : Controller
    {
        private readonly IDocumentStore store;

        public ApiResourcesController(IDocumentStore store)
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
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + document.Name),
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
                    return this.NotFound(new { Message = $"API resource '{resourceName}' not found" });
                }

                return this.Ok(
                    new ResourceResource
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + document.Name),
                        Name = document.Name,
                        DisplayName = document.DisplayName,
                        UserClaims = document.UserClaims?.Select(item => item.Type).ToList(),
                        ApiScopes = document.Scopes?
                            .Select(scope => new ResourceResource.Scope { Name = scope.Name, UserClaims = scope.UserClaims.Select(claim => claim.Type).ToList() })
                            .ToList(),
                        Enabled = document.Enabled,
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IroncladResource model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return this.BadRequest(new { Message = $"Cannot create an API resource without a name" });
            }

            if (string.IsNullOrEmpty(model.ApiSecret))
            {
                return this.BadRequest(new { Message = $"Cannot create an API resource without a secret" });
            }

            var resource = new IdentityServerResource(model.Name, model.DisplayName)
            {
                ApiSecrets = new List<Secret> { new Secret(model.ApiSecret.Sha256()) },
            };

            // optional properties
            resource.UserClaims = model.UserClaims ?? resource.UserClaims;
            resource.Scopes = model.ApiScopes?.Select(item => new Scope(item.Name, item.UserClaims)).ToList() ?? resource.Scopes;
            resource.Enabled = model.Enabled ?? resource.Enabled;

            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresResource>().Any(document => document.Name == model.Name))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "API resource already exists" });
                }

                session.Insert(resource.ToEntity());

                await session.SaveChangesAsync();
            }

            return this.Created(new Uri(this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + model.Name)), null);
        }

        [HttpPut("{resourceName}")]
        public async Task<IActionResult> Put(string resourceName, [FromBody]IroncladResource model)
        {
            using (var session = this.store.LightweightSession())
            {
                var document = await session.Query<PostgresResource>().SingleOrDefaultAsync(item => item.Name == resourceName);
                if (document == null)
                {
                    return this.NotFound(new { Message = $"API resource '{resourceName}' not found" });
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
