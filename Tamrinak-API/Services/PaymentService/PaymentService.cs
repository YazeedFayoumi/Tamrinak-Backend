using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.PaymentDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly IGenericRepo<Payment> _paymentRepo;
		private readonly IGenericRepo<User> _userRepo;
		private readonly IGenericRepo<Booking> _bookingRepo;
		private readonly IGenericRepo<Membership> _membershipRepo;


		public PaymentService(IGenericRepo<Payment> paymentRepo, IGenericRepo<User> userRepo)
		{
			_paymentRepo = paymentRepo;
			_userRepo = userRepo;
		}

		public async Task<PaymentDto> AddPaymentAsync(AddPaymentDto dto, string userEmail)
		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
					   ?? throw new Exception("User not found or unauthorized.");

			if (dto.BookingId.HasValue == dto.MembershipId.HasValue)
				throw new Exception("Payment must be linked to either a booking or a membership, not both or neither.");

			var payment = new Payment
			{
				Amount = dto.Amount,
				Method = dto.Method,
				TransactionId = dto.TransactionId,
				PaymentDate = DateTime.UtcNow,
			};

			if (dto.BookingId.HasValue)
			{
				var booking = await _bookingRepo.GetAsync(dto.BookingId.Value)
							  ?? throw new Exception("Booking not found.");

				if (booking.UserId != user.UserId)
					throw new UnauthorizedAccessException("You cannot pay for someone else's booking.");

				payment.BookingId = dto.BookingId.Value;
			}
			else if (dto.MembershipId.HasValue)
			{
				var membership = await _membershipRepo.GetAsync(dto.MembershipId.Value)
								 ?? throw new Exception("Membership not found.");

				if (membership.UserId != user.UserId)
					throw new UnauthorizedAccessException("You cannot pay for someone else's membership.");

				payment.MembershipId = dto.MembershipId.Value;
			}

			await _paymentRepo.AddAsync(payment);
			await _paymentRepo.SaveAsync();

			return new PaymentDto
			{
				PaymentId = payment.PaymentId,
				Amount = payment.Amount,
				Method = payment.Method,
				PaymentDate = payment.PaymentDate,
				TransactionId = payment.TransactionId
			};
		}

		public async Task<PaymentDto> GetPaymentByIdAsync(int paymentId)
		{
			var payment = await _paymentRepo.GetAsync(paymentId)
				?? throw new Exception("Payment not found");

			return new PaymentDto
			{
				PaymentId = payment.PaymentId,
				Amount = payment.Amount,
				PaymentDate = payment.PaymentDate,
				Method = payment.Method,
				IsConfirmed = payment.IsConfirmed,
				BookingId = payment.BookingId,
				MembershipId = payment.MembershipId,
				TransactionId = payment.TransactionId
			};
		}

		public async Task<List<PaymentDto>> GetUserPaymentsAsync(string userEmail)
		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
				?? throw new Exception("User not found");

			var payments = await _paymentRepo.GetListByConditionAsync(p =>
				(p.Booking != null && p.Booking.UserId == user.UserId) ||
				(p.Membership != null && p.Membership.UserId == user.UserId));

			return payments.Select(p => new PaymentDto
			{
				PaymentId = p.PaymentId,
				Amount = p.Amount,
				PaymentDate = p.PaymentDate,
				Method = p.Method,
				IsConfirmed = p.IsConfirmed,
				BookingId = p.BookingId,
				MembershipId = p.MembershipId,
				TransactionId = p.TransactionId
			}).ToList();
		}

		public async Task<bool> ConfirmPaymentAsync(int paymentId)
		{
			var payment = await _paymentRepo.GetAsync(paymentId)
				?? throw new Exception("Payment not found");

			if (payment.Method != PaymentMethod.Cash)
				throw new Exception("Only cash payments can be confirmed manually");

			payment.IsConfirmed = true;
			await _paymentRepo.UpdateAsync(payment);
			await _paymentRepo.SaveAsync();
			return true;
		}



	}
}
