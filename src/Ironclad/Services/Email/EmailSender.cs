// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Services.Email
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly string sender;
        private readonly string host;
        private readonly int port;
        private readonly bool enableSsl;
        private readonly string username;
        private readonly string password;

        public EmailSender(string sender, string host, int port, bool enableSsl, string username, string password)
        {
            this.sender = sender;
            this.host = host;
            this.port = port;
            this.enableSsl = enableSsl;
            this.username = username;
            this.password = password;
        }

        public Task SendEmailAsync(string email, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(this.host, this.port))
            using (var message = new MailMessage(this.sender, email))
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = this.enableSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(this.username, this.password);

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                smtpClient.Send(message);
            }

            return Task.CompletedTask;
        }
    }
}
