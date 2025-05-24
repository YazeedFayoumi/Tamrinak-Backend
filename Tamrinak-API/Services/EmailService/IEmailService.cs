using Tamrinak_API.DTO.AdminDtos;

namespace Tamrinak_API.Services.EmailService
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string body, string plainBody);
		Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
		Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
		Task SendMembershipExpiryReminderAsync(string toEmail, string userName, string facilityName, DateTime expirationDate);
		Task SendContactMessageAsync(ContactMessageDto dto);

    }
}
