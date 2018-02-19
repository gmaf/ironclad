// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Authorization
{
    using System;
    using Microsoft.AspNetCore.Authorization;

    public class AdministratorRequirement : IAuthorizationRequirement
    {
        public AdministratorRequirement(string type)
        {
            if (type != "auth" && type != "user")
            {
                throw new ArgumentException($"Invalid administrator requirement type: '{type}'.", nameof(type));
            }

            this.Type = type;
        }

        public string Type { get; private set; }
    }
}
