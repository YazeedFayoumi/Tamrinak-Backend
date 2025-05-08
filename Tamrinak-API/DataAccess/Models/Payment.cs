using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Payment
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PaymentId { get; set; }


		public int? BookingId { get; set; }
		public int? MembershipId { get; set; }

		[Required]
		[Column(TypeName = "decimal(10,2)")]
		public decimal Amount { get; set; }

		[Required]
		public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

		[Required]
		public PaymentMethod Method { get; set; }

		[Required]
		public PaymentStatus Status { get; set; }

		public string? TransactionId { get; set; }
		[Required]
		public bool IsConfirmed { get; set; } = false;

		public bool IsRefunded { get; set; } = false;

		public DateTime? CancelledAt { get; set; }
		public DateTime? RefundedAt { get; set; }
		public Booking? Booking { get; set; }
		public Membership? Membership { get; set; }
	}
	public enum PaymentMethod
	{
		Cash,
		Stripe,
		CliQ,
		Card
	}

	public enum PaymentStatus
	{
		Pending,
		Confirmed,
		Refunded,
		Cancelled,
		Failed
	}
}
