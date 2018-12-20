// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.ExternalIdentityProvider
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.AspNetCore.Authentication;

    public class IdentityProviderAuthenticationHandlerCache : IIdentityProviderAuthenticationHandlerCache
    {
        private readonly ConcurrentDictionary<string, IAuthenticationHandler> handlers = new ConcurrentDictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

        public bool TryGetValue(string authenticationScheme, out IAuthenticationHandler handler) => this.handlers.TryGetValue(authenticationScheme, out handler);

        public void AddOrUpdate(string authenticationScheme, IAuthenticationHandler handler) => this.handlers[authenticationScheme] = handler;

        public void TryRemove(string authenticationScheme) => this.handlers.TryRemove(authenticationScheme, out var _);
    }
}
