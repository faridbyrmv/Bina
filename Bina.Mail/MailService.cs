using MimeKit;
using MailKit.Net.Smtp;

namespace Bina.Mail
{
    public class MailService : IMailService
    {
        public async Task Send(string from, string to, string link, string subject)
        {
            // Set the path for the email template
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "Templates", "EmailTemplate.html");

            // Check if the template exists
            if (!File.Exists(path))
                throw new FileNotFoundException("Email template not found.", path);

            // Read the email template and replace the verification link
            string emailTemplate = await File.ReadAllTextAsync(path);
            emailTemplate = emailTemplate.Replace("{{verificationLink}}", link);

            // Create a MimeMessage object for the email
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            // Build the email body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailTemplate
            };
            email.Body = bodyBuilder.ToMessageBody();

            // Create and configure the SMTP client using MailKit
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.mail.ru", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("hacibalaev.azik@mail.ru", "hAY3Tcz8WgEh5Nryv1rF");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
