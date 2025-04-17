using System.Net.Mail;
using System.Net;

namespace Tamrinak_API.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_config["Email:SmtpHost"])
            {
                Port = int.Parse(_config["Email:SmtpPort"]),
                Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Email:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            string subject = "Confirm Your Email";
            string body = $"<p>Click the link below to confirm your email:</p><a href='{confirmationLink}'>Confirm Email</a>";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            string subject = "Reset Your Password";
            string body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
