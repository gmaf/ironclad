// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }

#pragma warning disable CA1056
        public string ReturnUrl { get; set; }
    }
}