using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Stripe.V2;
using System.Security.Claims;
using System.Text;
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

				var paymentId = await _paymentService.CreatePaymentAsync(userId, dto, false);
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
			try
			{
				var payment = await _paymentService.GetPaymentByIdAsync(id);
				return payment != null ? Ok(payment) : NotFound("Payment not found");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("my")]
		public async Task<IActionResult> GetMyPayments()
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? throw new Exception("User ID claim not found"));

				var payments = await _paymentService.GetPaymentsByUserAsync(userId);
				return Ok(payments);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("{id}/cancel")]
		public async Task<IActionResult> CancelPayment(int id)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? throw new Exception("User ID claim not found"));

				await _paymentService.CancelPaymentAsync(userId, id);
				return Ok("Payment cancelled.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("stripe/create-intent")]
		[Authorize]
		public async Task<IActionResult> CreateStripePaymentIntent([FromBody] StripeIntentRequestDto dto)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? throw new Exception("User ID not found"));

		
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
			var json = await new StreamReader(Request.Body).ReadToEndAsync();


			try
			{
				var stripeEvent = EventUtility.ConstructEvent(
					json,
					Request.Headers["Stripe-Signature"],
					_config["Stripe:WebhookSecret"]
				);

				
				// Call your service based on event type
				if (stripeEvent.Type == "checkout.session.completed")
				{
					var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

					if (session == null || string.IsNullOrEmpty(session.PaymentIntentId))
					{
						Console.WriteLine("⚠️ Session or PaymentIntentId is null.");
						return BadRequest();
					}

					var paymentIntentService = new PaymentIntentService();
					var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

					var userId = int.Parse(paymentIntent.Metadata["userId"]);
					var bookingId = int.Parse(paymentIntent.Metadata["bookingId"]);
					var amount = (decimal)(paymentIntent.AmountReceived / 100.0m);

					Console.WriteLine($"✅ Got metadata: userId={userId}, bookingId={bookingId}, amount={amount}");

					// continue with payment logic...



					var dto = new AddPaymentDto
					{
						BookingId = bookingId,
						Amount = amount,
						Method = DataAccess.Models.PaymentMethod.Stripe,
						TransactionId = paymentIntent.Id
					};

					var result = await _paymentService.CreatePaymentAsync(userId, dto, fromWebHook: true);
                    
                }
				else if(stripeEvent.Type == "payment_intent.payment_failed")
				{
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    var failureMessage = failedIntent?.LastPaymentError?.Message ?? "Unknown failure";
                    await _paymentService.HandleStripePaymentFailedAsync(failedIntent.Id, failureMessage);
                }
                return Ok();

            }
			catch (StripeException ex)
			{
				Console.WriteLine($"❌ Stripe webhook error: {ex.Message}");
				return BadRequest();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Webhook general error: {ex.Message}");
				return StatusCode(500);
			}
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
