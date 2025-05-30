using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.PaymentDtos;

namespace Tamrinak_API.Services.EmailService
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string body, string plainBody);
		Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
		Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
		Task SendMembershipExpiryReminderAsync(string toEmail, string userName, string facilityName, DateTime expirationDate);
		Task SendContactMessageAsync(ContactMessageDto dto);

		Task SendPaymentEmailAsync(string email, object emialInfo);

    }
}
