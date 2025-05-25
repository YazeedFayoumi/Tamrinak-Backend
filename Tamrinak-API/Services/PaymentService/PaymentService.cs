using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
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

        /*  public async Task<int> CreatePaymentAsync(int userId, AddPaymentDto dto)
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
                  p.BookingId == dto.BookingId.Value && p.IsConfirmed);

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
                   p.MembershipId == dto.MembershipId.Value && p.IsConfirmed);

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
  */
        public async Task<int> CreatePaymentAsync(int userId, AddPaymentDto dto, bool fromWebhook = false)
        {

            Console.WriteLine($"📌 Entered CreatePaymentAsync: userId={userId}, bookingId={dto.BookingId}, membershipId={dto.MembershipId}, amount={dto.Amount}");
            try
            {
                var user = await _userRepo.GetAsync(userId);
                if (user == null)
                {
                    /*if (fromWebhook)
                    {
                        Console.WriteLine($"⚠️ Webhook: User with ID {userId} not found. Skipping.");
                        return 0;
                    }*/
                    throw new Exception("User not found or unauthorized.");
                }

                if ((dto.BookingId.HasValue && dto.MembershipId.HasValue) || (!dto.BookingId.HasValue && !dto.MembershipId.HasValue))
                {
                   /* if (fromWebhook)
                    {
                        Console.WriteLine("⚠️ Webhook: Must provide either BookingId or MembershipId. Skipping.");
                        return 0;
                    }*/
                    throw new Exception("You must provide either a BookingId or MembershipId.");
                }

                decimal expectedAmount = 0;

                if (dto.BookingId.HasValue)
                {
                    var booking = await _bookingRepo.GetByConditionIncludeAsync(b => b.BookingId == dto.BookingId.Value, q=>q.Include(b=> b.User));
                    if (booking == null)
                    {
                        /*if (fromWebhook)
                        {
                            Console.WriteLine($"⚠️ Webhook: Booking {dto.BookingId} not found. Skipping.");
                            return 0;
                        }*/
                        throw new Exception("Booking not found.");
                    }

                    if (booking.UserId != userId)
                    {
                       /* if (fromWebhook)
                        {
                            Console.WriteLine($"❌ Booking belongs to {booking.UserId}, but metadata says {userId}");
                            return 0;
                        }*/
                        throw new Exception("You cannot pay for another user's booking.");
                    }

                    expectedAmount = booking.TotalCost;

                    var existingPayment = await _paymentRepo.GetByConditionAsync(p =>
                        p.BookingId == dto.BookingId); //&& p.IsConfirmed);
                    Console.WriteLine($"Checked existing confirmed payment for booking {dto.BookingId}: {existingPayment != null}");

                    if (existingPayment != null)
                    {
                        /*if (fromWebhook)
                        {
                            Console.WriteLine($"⚠️ Webhook: Booking {dto.BookingId} already has a confirmed payment. Skipping.");
                            return existingPayment.PaymentId; // Make sure this exists!
                        }*/
                        Console.WriteLine("‼️ Booking already has a confirmed payment. Throwing exception.");
                        throw new Exception("This booking already has a confirmed payment.");
                    }

                    Console.WriteLine("🧭 No existing confirmed payment found. Proceeding...");
                }
                else if (dto.MembershipId.HasValue)
                {
                    var membership = await _membershipRepo.GetAsync(dto.MembershipId.Value);
                    if (membership == null)
                    {
                        /*if (fromWebhook)
                        {
                            Console.WriteLine($"⚠️ Webhook: Membership {dto.MembershipId} not found. Skipping.");
                            return 0;
                        }*/
                        throw new Exception("Membership not found.");
                    }

                    if (membership.UserId != userId)
                    {
                       /* if (fromWebhook)
                        {
                            Console.WriteLine($"⚠️ Webhook: Membership {dto.MembershipId} does not belong to user {userId}. Skipping.");
                            return 0;
                        }*/
                        throw new Exception("You cannot pay for another user's membership.");
                    }

                    expectedAmount = membership.TotalOfferPaid ?? membership.MonthlyFee;

                    var existingPayment = await _paymentRepo.GetByConditionAsync(p =>
                        p.MembershipId == dto.MembershipId.Value);// && p.IsConfirmed);

                    if (existingPayment != null)
                    {
                       /* if (fromWebhook)
                        {
                            Console.WriteLine($"⚠️ Webhook: Membership {dto.MembershipId} already has a confirmed payment. Skipping.");
                            return existingPayment.PaymentId;
                        }*/
                        throw new Exception("This membership already has a confirmed payment.");
                    }
                }

                Console.WriteLine($"🔍 Expected: {expectedAmount}, Provided: {dto.Amount}");
                if (decimal.Round(dto.Amount, 2) != decimal.Round(expectedAmount, 2))
                {
                   /* if (fromWebhook)
                    {
                        Console.WriteLine($"❌ Payment amount mismatch: expected {expectedAmount}, got {dto.Amount}");
                        return 0;
                    }*/
                    throw new Exception("Incorrect payment amount.");
                }

        /*        if (dto.Method == PaymentMethod.Stripe && string.IsNullOrEmpty(dto.TransactionId))
                {
                    if (fromWebhook)
                    {
                        Console.WriteLine("⚠️ Webhook: Stripe transaction ID missing. Skipping.");
                        return 0;
                    }
                    throw new Exception("Stripe payment requires a transaction ID.");
                }*/

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

                Console.WriteLine($"✅ Payment created successfully for user {userId}. PaymentId: {payment.PaymentId}");

                return payment.PaymentId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in CreatePaymentAsync: {ex.Message}");
               // if (fromWebhook) return 0;
                throw;
            }
          
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

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = $"{_config["ClientApp:BaseUrl"]}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_config["ClientApp:BaseUrl"]}/payment-cancelled",
                LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = dto.Currency,
                                UnitAmount = (dto.Amount * 100),        

                            ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Tamrinak Payment"
                                }
                            },
                            Quantity = 1
                        }
                    },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                        {
                            { "userId", userId.ToString() },
                            { "bookingId", dto.BookingId?.ToString() ?? string.Empty },
                            { "membershipId", dto.MembershipId?.ToString() ?? string.Empty },
                            { "amount", dto.Amount.ToString() },
                            { "currency", dto.Currency }
                        }
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

        public async Task HandleStripePaymentFailedAsync(string transactionId, string failureReason)
        {
            var payment = await _paymentRepo.GetByConditionAsync(p => p.TransactionId == transactionId);
            if (payment == null) return;

            payment.Status = PaymentStatus.Failed;
           // payment.FailureReason = failureReason;
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
