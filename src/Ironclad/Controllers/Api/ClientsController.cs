// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Postgresql.Mappers;
    using Marten;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PostgresClient = IdentityServer4.Postgresql.Entities.Client;

    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private readonly IDocumentStore store;

        public ClientsController(IDocumentStore store)
        {
            this.store = store;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            using (var session = this.store.LightweightSession())
            {
                var clients = await session.Query<PostgresClient>()
                    .ToListAsync();

                return this.Ok(
                    clients.Select(
                        item =>
                        new
                        {
                            Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/clients/" + item.ClientId),
                            item.ClientId,
                            item.ClientName,
                            item.Enabled,
                        }));
            }
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> Get(string clientId)
        {
            using (var session = this.store.LightweightSession())
            {
                var client = await session.Query<PostgresClient>()
                    .SingleOrDefaultAsync(item => item.ClientId == clientId);

                if (client == null)
                {
                    return this.NotFound();
                }

                return this.Ok(
                    new
                    {
                        Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/clients/" + client.ClientId),
                        client.ClientId,
                        client.ClientName,
                        AllowedCorsOrigins = client.AllowedCorsOrigins.Select(item => item.Origin).ToArray(),
                        RedirectUris = client.RedirectUris.Select(item => item.RedirectUri).ToArray(),
                        PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(item => item.PostLogoutRedirectUri).ToArray(),
                        AllowedScopes = client.AllowedScopes.Select(item => item.Scope).ToArray(),
                        AccessTokenType = ((AccessTokenType)client.AccessTokenType).ToString(),
                        client.Enabled,
                    });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CustomClient model)
        {
            // NOTE (Cameron): We create a single client secret from the passed secret property. This is by design.
            model.ClientSecrets = new List<Secret> { new Secret(model.Secret.Sha256()) };

            using (var session = this.store.LightweightSession())
            {
                if (session.Query<PostgresClient>().Any(client => client.ClientId == model.ClientId))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "Client already exists" });
                }

                session.Insert(model.ToEntity());

                await session.SaveChangesAsync();
            }

            this.Response.Headers.Add("Location", this.HttpContext.GetIdentityServerRelativeUrl("~/api/clients/" + model.ClientId));

            return this.Ok();
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> Put(string clientId, [FromBody]CustomClient model)
        {
            model.ClientId = clientId;
            model.ClientSecrets = new List<Secret> { new Secret(model.Secret.Sha256()) };

            using (var session = this.store.LightweightSession())
            {
                var client = await session.Query<PostgresClient>().SingleOrDefaultAsync(item => item.ClientId == model.ClientId);
                if (client == null)
                {
                    return this.NotFound(new { Message = "Client not found" });
                }

                var entity = model.ToEntity();
                entity.Id = client.Id;

                session.Store(entity);

                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete(string clientId)
        {
            using (var session = this.store.LightweightSession())
            {
                session.DeleteWhere<PostgresClient>(item => item.ClientId == clientId);
                await session.SaveChangesAsync();
            }

            return this.Ok();
        }

#pragma warning disable CA1034
        public class CustomClient : Client
        {
            public string Secret { get; set; }
        }
    }
}
