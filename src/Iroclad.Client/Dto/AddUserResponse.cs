// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    using Newtonsoft.Json;

    internal class AddUserResponse
    {
        [JsonProperty("registration_link")]
        public string RegistrationLink { get; set; }
    }
}
