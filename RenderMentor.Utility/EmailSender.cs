using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace RenderMentor.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions emailOptions;

        public EmailSender(IOptions<EmailOptions> options)
        {
            emailOptions = options.Value;
        }
        //public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    var client = new System.Net.Mail.SmtpClient(emailOptions.Host, emailOptions.Port)
        //    {
        //        Credentials = new System.Net.NetworkCredential(emailOptions.mailAddress, emailOptions.Password),
        //        EnableSsl = emailOptions.EnableSSL
        //    };
        //    return client.SendMailAsync(
        //        new System.Net.Mail.MailMessage(emailOptions.mailAddress, email, subject, htmlMessage)
        //        {
        //            IsBodyHtml = true
        //        }
        //    );
        //}
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("", emailOptions.mailAddress));
            mail.To.Add(new MailboxAddress("", email));

            mail.Subject = subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            var smtp = new SmtpClient();
            smtp.Connect(emailOptions.Host, emailOptions.Port, false);

            smtp.Authenticate(emailOptions.mailAddress, emailOptions.Password);

            smtp.Send(mail);
            smtp.Disconnect(true);
            return Task.CompletedTask;
        }
    }
}
