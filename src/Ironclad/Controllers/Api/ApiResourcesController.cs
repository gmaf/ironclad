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
    using PostgresApiResource = IdentityServer4.Postgresql.Entities.ApiResource;

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
                var totalSize = await session.Query<PostgresApiResource>().CountAsync();
                var apiResources = await session.Query<PostgresApiResource>().Skip(skip).Take(take).ToListAsync();

                var resources = apiResources.Select(item =>
                new ApiResourceSummaryResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + item.Name),
                    Name = item.Name,
                    DisplayName = item.DisplayName,
                    Enabled = item.Enabled
                });

                var resourceSet = new ResourceSet<ApiResourceSummaryResource>(skip, totalSize, resources);

                return this.Ok(resourceSet);

                /*
                 * Name
                 * DisplayName
                 * Scopes
                 * UserClaims
                 * Secrets
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
                var totalSize = await session.Query<PostgresApiResource>().CountAsync();
                var apiResource = await session.Query<PostgresApiResource>()
                    .SingleOrDefaultAsync(item => item.Name == name);

                if (apiResource == null)
                {
                    return this.NotFound();
                }

                return this.Ok(
                    new
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + apiResource.Name),
                        apiResource.Name,
                        apiResource.DisplayName,
                        ScopeNames = apiResource.Scopes?.Select(item => item.Name),
                        UserClaims = apiResource.UserClaims?.Select(item => item.Type).ToArray(),
                        apiResource.Description,
                        apiResource.Enabled
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CustomApiResource model)
        {
            model.ApiSecrets = new List<Secret> { new Secret(model.Secret.Sha256()) };
            model.Scopes = model.ScopeNames?.Select(item => new Scope(item)).ToList();

            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresApiResource>().Any(client => client.Name == model.Name))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Api resource already exists" });
                }

                session.Insert(model.ToEntity());

                await session.SaveChangesAsync();
            }

            this.Response.Headers.Add("Location", this.HttpContext.GetIdentityServerRelativeUrl("~/api/apiresources/" + model.Name));

            return this.Ok();
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody]CustomApiResource model)
        {
            model.Name = name;
            model.ApiSecrets = new List<Secret> { new Secret(model.Secret.Sha256()) };
            model.Scopes = model.ScopeNames?.Select(item => new Scope(item)).ToList();

            using (var session = this.store.LightweightSession())
            {
                var apiResource = await session.Query<PostgresApiResource>().SingleOrDefaultAsync(item => item.Name == model.Name);
                if (apiResource == null)
                {
                    return this.NotFound(new { Message = "Api resource not found" });
                }

                var entity = model.ToEntity();
                entity.Id = apiResource.Id;

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
                session.DeleteWhere<PostgresApiResource>(item => item.Name == name);
                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

#pragma warning disable CA1034
        public class CustomApiResource : ApiResource
        {
            public string Secret { get; set; }

            public ICollection<string> ScopeNames { get; set; }
        }

        private class ApiResourceSummaryResource : ResourceSummary
        {
            public string Url { get; set; }
        }
    }
}
