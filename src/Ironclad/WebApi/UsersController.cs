// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4.Extensions;
    using Ironclad.Application;
    using Ironclad.Client;
    using Ironclad.Configuration;
    using Ironclad.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize("user_admin")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender emailSender;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string username, int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            var userQuery = string.IsNullOrEmpty(username)
                ? this.userManager.Users
                : this.userManager.Users.Where(user => user.UserName.StartsWith(username, StringComparison.OrdinalIgnoreCase));

            var totalSize = await userQuery.CountAsync();
            var users = await userQuery.OrderBy(user => user.UserName).Skip(skip).Take(take).ToListAsync();
            var resources = users.Select(
                user =>
                new UserSummaryResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/users/" + user.UserName),
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                });

            var resourceSet = new ResourceSet<UserSummaryResource>(skip, totalSize, resources);

            return this.Ok(resourceSet);
        }

        [HttpHead("{username}")]
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null)
            {
                return this.NotFound(new { Message = $"User '{username}' not found" });
            }

            var roles = await this.userManager.GetRolesAsync(user);

            return this.Ok(
                new UserResource
                {
                    Url = this.HttpContext.GetIdentityServerRelativeUrl("~/api/users/" + user.UserName),
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = new List<string>(roles),
                });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User model, bool sendEmail = true)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                return this.BadRequest(new { Message = $"Cannot create a user without a username" });
            }

            var user = new ApplicationUser(model.Username);

            // optional properties
            user.Email = model.Email ?? user.Email;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            if (model.Roles != null)
            {
                foreach (var role in model.Roles)
                {
                    if (!await this.roleManager.RoleExistsAsync(role))
                    {
                        return this.BadRequest(new { Message = $"Cannot create a user with the role '{role}' when that role does not exist" });
                    }
                }
            }

            var addUserResult = string.IsNullOrEmpty(model.Password) ? await this.userManager.CreateAsync(user) : await this.userManager.CreateAsync(user, model.Password);
            if (!addUserResult.Succeeded)
            {
                if (addUserResult.Errors.Any(error => error.Code == "DuplicateUserName"))
                {
                    return this.StatusCode((int)HttpStatusCode.Conflict, new { Message = "User already exists" });
                }

                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = addUserResult.ToString() });
            }

            if (!string.IsNullOrEmpty(model.ExternalLoginProvider))
            {
                var claims = new[] { new Claim(JwtClaimTypes.Name, model.Username) };
                var identity = new ClaimsIdentity(claims, model.ExternalLoginProvider, "name", "role");
                var principal = new ClaimsPrincipal(identity);
                var info = new ExternalLoginInfo(principal, model.ExternalLoginProvider, model.Username, model.Username);

                var addLoginResult = await this.userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = addLoginResult.ToString() });
                }
            }

            if (model.Roles != null)
            {
                var addToRolesResult = await this.userManager.AddToRolesAsync(user, model.Roles);
                if (!addToRolesResult.Succeeded)
                {
                    return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = addToRolesResult.ToString() });
                }
            }

            string callbackUrl = null;

            if (string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.Email))
            {
                var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
                callbackUrl = this.Url.CompleteRegistrationLink(user.Id, code, this.Request.Scheme);
                if (sendEmail)
                {
                    await this.emailSender.SendActivationEmailAsync(model.Email, callbackUrl);
                }
            }

            return this.Created(
                new Uri(this.HttpContext.GetIdentityServerRelativeUrl("~/api/users/" + model.Username)),
                callbackUrl != null ? new { registrationLink = callbackUrl } : null);
        }

        [HttpPut("{username}")]
        public async Task<IActionResult> Put(string username, [FromBody]User model)
        {
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null)
            {
                return this.NotFound(new { Message = $"User '{username}' not found" });
            }

            if (user.Id == Config.DefaultAdminUserId && model.Roles != null && !model.Roles.Contains("admin"))
            {
                return this.BadRequest(new { Message = $"Cannot remove the role 'admin' from the default admin user" });
            }

            user.UserName = model.Username ?? user.UserName;
            user.Email = model.Email ?? user.Email;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = result.ToString() });
            }

            var roles = await this.userManager.GetRolesAsync(user);

            var oldRoles = roles.Except(model.Roles ?? Array.Empty<string>());
            if (oldRoles.Any())
            {
                var removeResult = await this.userManager.RemoveFromRolesAsync(user, oldRoles);
                if (!removeResult.Succeeded)
                {
                    return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = removeResult.ToString() });
                }
            }

            var newRoles = (model.Roles ?? Array.Empty<string>()).Except(roles);
            if (newRoles.Any())
            {
                var addResult = await this.userManager.AddToRolesAsync(user, newRoles);
                if (!addResult.Succeeded)
                {
                    return this.StatusCode((int)HttpStatusCode.InternalServerError, new { Message = addResult.ToString() });
                }
            }

            return this.Ok();
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null)
            {
                return this.Ok();
            }

            if (user.Id == Config.DefaultAdminUserId)
            {
                return this.BadRequest(new { Message = $"Cannot remove the default admin user" });
            }

            await this.userManager.DeleteAsync(user);

            return this.Ok();
        }

#pragma warning disable CA1034, CA1056
        private class UserResource : User
        {
            public string Url { get; set; }
        }

        private class UserSummaryResource : UserSummary
        {
            public string Url { get; set; }
        }
    }
}
