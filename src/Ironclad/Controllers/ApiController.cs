// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Authorize("admin")]
    public class ApiController
    {
        // manage clients client
        // list, add, remove

        // manage identity resources
        // list, add, remove

        // manage api resources
        // list, add, remove
    }
}
