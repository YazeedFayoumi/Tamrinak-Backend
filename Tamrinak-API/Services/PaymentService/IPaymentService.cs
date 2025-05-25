using Stripe;
using Tamrinak_API.DTO.PaymentDtos;

namespace Tamrinak_API.Services.PaymentService
{
	public interface IPaymentService
	{
        Task<int> CreatePaymentAsync(int userId, AddPaymentDto dto, bool fromWebHook);
        Task<PaymentDto?> GetPaymentByIdAsync(int paymentId);
        Task<List<PaymentDto>> GetPaymentsByUserAsync(int userId);
        Task CancelPaymentAsync(int userId, int paymentId);

        Task<string> CreateStripeIntentAsync(int userId, StripeIntentRequestDto dto);
        Task MarkStripePaymentAsPaidAsync(string transactionId);
        Task HandleStripePaymentFailedAsync(string transactionId, string failureReason);
    }
}
