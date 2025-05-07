using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO.PaymentDtos;
using Tamrinak_API.Services.PaymentService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) 
        {
            _paymentService = paymentService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddPayment([FromBody] AddPaymentDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _paymentService.AddPaymentAsync(dto, email);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetUserPayments()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _paymentService.GetUserPaymentsAsync(email);
            return Ok(result);
        }

        [HttpPut("{id}/confirm")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var result = _paymentService.ConfirmPaymentAsync(id);
            return Ok(result);
        }

    }
}
