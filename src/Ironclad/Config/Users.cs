// <copyright file="Users.cs" company="Lykke">
// Copyright (c) Ironclad Contributors. All rights reserved.
// </copyright>

namespace Ironclad.Config
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using IdentityModel;
    using IdentityServer4.Test;

    internal class Users
    {
        public static List<TestUser> GetTestUsers() =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "af4ecfd06dc4489ead44c0b3aa639b11",
                    Username = "cameron",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "cameron.fletcher@lykke.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
    }
}
