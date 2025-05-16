using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

using System.Security.Claims;
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
        public async Task<IActionResult> CreateStripePaymentIntent([FromBody] StripeIntentRequestDto dto)
        {
            var intent = await _paymentService.CreateStripeIntentAsync(dto.Amount, dto.Currency);
            return Ok(new { clientSecret = intent.ClientSecret });
        }

        // Webhook (Stripe -> you)
       
        [HttpPost("stripe/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var secret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, secret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            // ✅ Use the correct constant or string literal
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                if (intent == null)
                    return BadRequest("Invalid PaymentIntent data.");

                var transactionId = intent.Id;

                await _paymentService.MarkStripePaymentAsPaidAsync(transactionId);
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
