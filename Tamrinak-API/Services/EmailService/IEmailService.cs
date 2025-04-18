namespace Tamrinak_API.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string plainBody);
        Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
