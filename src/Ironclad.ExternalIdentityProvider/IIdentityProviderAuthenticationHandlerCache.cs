// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.ExternalIdentityProvider
{
    using Microsoft.AspNetCore.Authentication;

    public interface IIdentityProviderAuthenticationHandlerCache
    {
        bool TryGetValue(string authenticationScheme, out IAuthenticationHandler handler);

        void AddOrUpdate(string authenticationScheme, IAuthenticationHandler handler);

        void TryRemove(string authenticationScheme);
    }
}
