using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.PaymentDtos;
using Tamrinak_API.Services.PaymentService;

namespace Tamrinak_API.Controllers
{
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
		public PaymentController(IPaymentService paymentService, IConfiguration configuration)
		{
			_paymentService = paymentService;
            _config = configuration;
		}

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] AddPaymentDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID claim not found"));

                var paymentId = await _paymentService.CreatePaymentAsync(userId, dto);
                return Ok(new { PaymentId = paymentId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            return payment != null ? Ok(payment) : NotFound("Payment not found");
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPayments()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new Exception("User ID claim not found"));

            var payments = await _paymentService.GetPaymentsByUserAsync(userId);
            return Ok(payments);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelPayment(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new Exception("User ID claim not found"));

            await _paymentService.CancelPaymentAsync(userId, id);
            return Ok("Payment cancelled.");
        }

        [HttpPost("stripe/create-intent")]
        //[Authorize]
        public async Task<IActionResult> CreateStripePaymentIntent([FromBody] StripeIntentRequestDto dto)
        {
            try
            {
                // int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                //  ?? throw new Exception("User ID not found"));
                dto.MembershipId =null;
                var sessionId = await _paymentService.CreateStripeIntentAsync(12, dto);
                return Ok(new { sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Invalid Stripe Signature: {ex.Message}");
                return BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                try
                {
                    Console.WriteLine("✅ Handling 'checkout.session.completed'");

                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    Console.WriteLine("➡️ Session ID: " + session?.Id);

                    var metadata = session.Metadata;
                    foreach (var kv in metadata)
                        Console.WriteLine($"🔖 {kv.Key}: {kv.Value}");

                    // Parse metadata
                    int userId = int.Parse(metadata["userId"]);
                    int amount = int.Parse(metadata["amount"]);
                    int? bookingId = metadata.TryGetValue("bookingId", out var bStr) && int.TryParse(bStr, out var b) ? b : null;
                    int? membershipId = metadata.TryGetValue("membershipId", out var mStr) && int.TryParse(mStr, out var m) ? m : null;

                    Console.WriteLine($"👤 userId={userId}, 💵 amount={amount}, 🏷️ bookingId={bookingId}, membershipId={membershipId}");

                    var dto = new AddPaymentDto
                    {
                        Amount = amount,
                        Method = DataAccess.Models.PaymentMethod.Stripe,
                        BookingId = bookingId,
                        MembershipId = membershipId,
                        TransactionId = session.Id
                    };

                    Console.WriteLine("📥 Calling CreatePaymentAsync...");

                    await _paymentService.CreatePaymentAsync(userId, dto);

                    Console.WriteLine("✅ Payment saved to DB");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Webhook logic failed: {ex.Message}");
                    Console.WriteLine(ex.StackTrace); // very important!
                    return StatusCode(500, "Webhook failure: " + ex.Message);
                }
            }

            return Ok();
        }

        /*		[HttpPut("{id}/confirm")]
            [Authorize(Roles = "Admin,SuperAdmin")]
            public async Task<IActionResult> ConfirmPayment(int id)
            {
                var result = _paymentService.ConfirmPaymentAsync(id);
                return Ok(result);
            }*/

    }
}
