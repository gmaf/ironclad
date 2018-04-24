// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad
{
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Ironclad.Services;

    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
