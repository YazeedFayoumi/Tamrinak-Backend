using Google.Apis.Gmail.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using MimeKit;
using Google.Apis.Gmail.v1.Data;

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
   
                var receiver = new LocalServerCodeReceiver( "http://127.0.0.1:5005/authorize/");
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
            <p>Hello,</p>
            <p>Thanks for registering with Tamrinak! Please confirm your email address by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Confirm My Email</a></p>
            <p>If you didn’t request this, you can safely ignore this email.</p>
            <p>– The Tamrinak Team</p>
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
    }
}
