using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.PaymentDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;
using PaymentMethod = Tamrinak_API.DataAccess.Models.PaymentMethod;

namespace Tamrinak_API.Services.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly IGenericRepo<Payment> _paymentRepo;
		private readonly IGenericRepo<User> _userRepo;
		private readonly IGenericRepo<Booking> _bookingRepo;
		private readonly IGenericRepo<Membership> _membershipRepo;
        private readonly IConfiguration _config;

		public PaymentService(IGenericRepo<Payment> paymentRepo, IGenericRepo<User> userRepo, IConfiguration configuration,
           IGenericRepo<Booking> bookingRepo, IGenericRepo<Membership> membershipRepo )
		{
			_paymentRepo = paymentRepo;
			_userRepo = userRepo;
            _bookingRepo = bookingRepo;
            _membershipRepo = membershipRepo;
            _config = configuration;
		}

        public async Task<int> CreatePaymentAsync(int userId, AddPaymentDto dto)
        {
            var user = await _userRepo.GetAsync(userId)
                       ?? throw new Exception("User not found or unauthorized.");

            if ((dto.BookingId.HasValue && dto.MembershipId.HasValue) || (!dto.BookingId.HasValue && !dto.MembershipId.HasValue))
                throw new Exception("You must provide either a BookingId or MembershipId.");

            decimal expectedAmount = 0;

            if (dto.BookingId.HasValue)
            {
                var booking = await _bookingRepo.GetAsync(dto.BookingId.Value)
                    ?? throw new Exception("Booking not found.");

                if (booking.UserId != userId)
                    throw new Exception("You cannot pay for another user's booking.");

                expectedAmount = booking.TotalCost;

                var existingPayment = await _paymentRepo.GetByConditionAsync(p =>
                p.BookingId == dto.BookingId.Value); // && p.IsConfirmed);

                if (existingPayment != null)
                    throw new Exception("This booking already has a confirmed payment.");

            }
            else if (dto.MembershipId.HasValue)
            {
                var membership = await _membershipRepo.GetAsync(dto.MembershipId.Value)
                    ?? throw new Exception("Membership not found.");

                if (membership.UserId != userId)
                    throw new Exception("You cannot pay for another user's membership.");

                expectedAmount = membership.TotalOfferPaid ?? membership.MonthlyFee;

                var existingPayment = await _paymentRepo.GetByConditionAsync(p =>
                 p.MembershipId == dto.MembershipId.Value); //&& p.IsConfirmed);

                if (existingPayment != null)
                    throw new Exception("This membership already has a confirmed payment.");
            }

            if (dto.Amount != expectedAmount)
                throw new Exception("Incorrect payment amount.");


            if (dto.Method == PaymentMethod.Stripe && string.IsNullOrEmpty(dto.TransactionId))
                throw new Exception("Stripe payment requires a transaction ID.");

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                MembershipId = dto.MembershipId,
                Amount = dto.Amount,
                Method = dto.Method,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow,
                TransactionId = dto.TransactionId,
                IsConfirmed = false
            };

            await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveAsync();

            return payment.PaymentId;
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetAsync(paymentId);
            if (payment == null) return null;

            return ToDto(payment);
        }

        public async Task<List<PaymentDto>> GetPaymentsByUserAsync(int userId)
        {
            var payments = await _paymentRepo.GetListByConditionIncludeAsync(
                p => (p.Booking != null && p.Booking.UserId == userId) || (p.Membership != null && p.Membership.UserId == userId),
                q => q.Include(p => p.Booking)
                      .Include(p => p.Membership)
            );

            return payments.Select(ToDto).ToList();
        }

        public async Task CancelPaymentAsync(int userId, int paymentId)
        {
            var payment = await _paymentRepo.GetByConditionIncludeAsync(
                p => p.PaymentId == paymentId &&
                    ((p.Booking != null && p.Booking.UserId == userId) ||
                     (p.Membership != null && p.Membership.UserId == userId)),
                q => q.Include(p => p.Booking)
                      .Include(p => p.Membership)
            );

            if (payment == null)
                throw new Exception("Payment not found");

            if (payment.IsConfirmed || payment.IsRefunded)
                throw new Exception("Cannot cancel a confirmed or refunded payment.");

            payment.Status = PaymentStatus.Cancelled;
            payment.CancelledAt = DateTime.UtcNow;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveAsync();
        }

        public async Task<string> CreateStripeIntentAsync(int userId, StripeIntentRequestDto dto)
        {
   /*         StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses smallest currency unit (e.g., 100 JOD = 10000)
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);*/

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                   
                    Currency = dto.Currency,
                    UnitAmount = dto.Amount,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Tamrinak Payment"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{_config["ClientApp:BaseUrl"]}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_config["ClientApp:BaseUrl"]}/payment-cancelled",
                Metadata = new Dictionary<string, string>
        {
            { "userId", userId.ToString() },
            { "bookingId", dto.BookingId?.ToString() ?? string.Empty },
            { "membershipId", dto.MembershipId?.ToString() ?? string.Empty },
            { "amount", dto.Amount.ToString() },
            { "currency", dto.Currency }
        }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Id;
        }

        public async Task MarkStripePaymentAsPaidAsync(string transactionId)
        {
            var payment = await _paymentRepo.GetByConditionAsync(p =>
                p.TransactionId == transactionId && p.Method == PaymentMethod.Stripe);

            if (payment == null)
                throw new Exception("Stripe payment not found.");

            payment.Status = PaymentStatus.Paid; // still pending manual confirmation
            payment.IsConfirmed = false;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveAsync();
        }


        private PaymentDto ToDto(Payment p)
        {
            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                ForType = p.BookingId.HasValue ? "Booking" : "Membership",
                ReferenceId = p.BookingId ?? p.MembershipId ?? 0,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.Method.ToString(),
                Status = p.Status.ToString(),
                IsConfirmed = p.IsConfirmed,
                IsRefunded = p.IsRefunded
            };
        }

    }
}
