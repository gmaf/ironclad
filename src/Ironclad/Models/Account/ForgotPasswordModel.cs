// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
