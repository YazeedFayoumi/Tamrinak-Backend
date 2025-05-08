using Tamrinak_API.DTO.PaymentDtos;

namespace Tamrinak_API.Services.PaymentService
{
	public interface IPaymentService
	{
		Task<PaymentDto> AddPaymentAsync(AddPaymentDto dto, string userEmail);
		Task<bool> ConfirmPaymentAsync(int paymentId); // Admin-only
		Task<List<PaymentDto>> GetUserPaymentsAsync(string userEmail);
		Task<PaymentDto> GetPaymentByIdAsync(int paymentId);


	}
}
