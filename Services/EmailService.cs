namespace ZentechAPI.Services
{
    using System.Net;
    using System.Net.Mail;
    using ZentechAPI.Models;

    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            using var smtpClient = new SmtpClient(_emailSettings.SMTPServer, _emailSettings.SMTPPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SMTPUsername, _emailSettings.SMTPPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipientEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
