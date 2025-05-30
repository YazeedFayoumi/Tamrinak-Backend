using Google.Apis.Gmail.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using MimeKit;
using Google.Apis.Gmail.v1.Data;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.PaymentDtos;
using Stripe.V2;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.Services.EmailService
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _config;
		private readonly GmailService _gmailService;

		public EmailService(IConfiguration config)
		{
			_config = config;

			UserCredential credential;
			using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{

				var receiver = new LocalServerCodeReceiver("http://127.0.0.1:5005/authorize/");
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
				  GoogleClientSecrets.FromStream(stream).Secrets,
				  new[] { GmailService.Scope.GmailSend },
				  "user",
				  CancellationToken.None,
				  new FileDataStore("token.json", true),
				  receiver
			  ).Result;
			}

			_gmailService = new GmailService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "Tamrinak Mailer"
			});
		}

		public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string plainTextBody = "")
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Tamrinak Team", "noreply@tamrinak.com"));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;

			var builder = new BodyBuilder
			{
				HtmlBody = htmlBody,
				TextBody = plainTextBody
			};
			message.Body = builder.ToMessageBody();

			using (var stream = new MemoryStream())
			{
				await message.WriteToAsync(stream);
				var rawMessage = Convert.ToBase64String(stream.ToArray())
					.Replace("+", "-").Replace("/", "_").Replace("=", "");

				var gmailMessage = new Message
				{
					Raw = rawMessage
				};

				await _gmailService.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
			}
		}

		public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
		{
			string subject = "Please Confirm Your Email Address";
			string htmlBody = $@"
            <html>
              <body>
                <p>Hello,</p>
                <p>Thanks for registering with Tamrinak! Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}' style='background-color:#4CAF50;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>Confirm My Email</a></p>
                <p>If you didn’t request this, you can safely ignore this email.</p>
                <p>– The Tamrinak Team</p>
              </body>
            </html>";


			string plainTextBody = $"Hello,\n\nThanks for registering with Tamrinak! Please confirm your email by clicking the link below:\n{confirmationLink}\n\n– The Tamrinak Team";

			await SendEmailAsync(toEmail, subject, htmlBody, plainTextBody);
		}

		public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
		{

			string subject = "Reset Your Tamrinak Password";
			string htmlBody = $@"
            <html>
            <p>Hello,</p>
            <p>We received a request to reset your Tamrinak password. You can reset it using the link below:</p>
            <p><a href='{resetLink}'>Reset My Password</a></p>
            <p>If you didn’t request a password reset, no further action is required.</p>
            <p>– The Tamrinak Team</p>
            </html>";

			string plainTextBody = $"Hello,\n\nWe received a request to reset your Tamrinak password. Use the link below to reset it:\n{resetLink}\n\nIf you didn’t request this, ignore this email.\n\n– The Tamrinak Team";

			await SendEmailAsync(toEmail, subject, htmlBody, plainTextBody);
		}

        public async Task SendMembershipExpiryReminderAsync(string toEmail, string userName, string facilityName, DateTime expirationDate)
        {
            string subject = "Membership Expiry Reminder";
            string htmlBody = $@"
			<p>Hello {userName},</p>
			<p>Your membership at <strong>{facilityName}</strong> will expire on <strong>{expirationDate:yyyy-MM-dd}</strong>.</p>
			<p>Please renew to continue enjoying our services.</p>
			<p>– The Tamrinak Team</p>";

            string plainTextBody = $"Hello {userName},\n\nYour membership at {facilityName} will expire on {expirationDate:yyyy-MM-dd}.\n\n– The Tamrinak Team";

            await SendEmailAsync(toEmail, subject, htmlBody, plainTextBody);
        }

        public async Task SendContactMessageAsync(ContactMessageDto dto)
        {
            var subject = $"New Contact Message from {dto.Name}";
            var body = $@"
			<h3>New message from Tamrinak contact form</h3>
			<p><strong>Name:</strong> {dto.Name}</p>
			<p><strong>Email:</strong> {dto.Email}</p>
			<p><strong>Message:</strong></p>
			<p>{dto.Message}</p>";

            await SendEmailAsync("yazeed.fayoumi@gmail.com", subject, body); 
        }

        public async Task SendPaymentEmailAsync(string email, object emailInfo)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email address is required", nameof(email));

            // Use dynamic typing to access properties
            dynamic info = emailInfo;

            string subject = "Payment Confirmation - Tamrinak";

            var body = $@"
				<h3>✅ Payment Confirmation</h3>
				<p>Dear user,</p>

				<p>We’re pleased to inform you that your payment was successfully processed. Here are the details:</p>

				<p><strong> Venue:</strong> {info.VenueName}</p>
				<p><strong> Event Date:</strong> {((DateTime)info.EventDate).ToString("yyyy-MM-dd")}</p>
				<p><strong> Payment Date:</strong> {((DateTime)info.PaymentDate).ToString("yyyy-MM-dd HH:mm")}</p>
				<p><strong> Amount Paid:</strong> {info.AmountPayed:F2} JOD</p>
				<p><strong> Payment Method:</strong> {info.MethodUsed}</p>

				<p>Thank you for choosing <strong>Tamrinak</strong>!<br />
				We look forward to serving you again.</p>

				<p>Best regards,<br />
				Tamrinak Team</p>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
