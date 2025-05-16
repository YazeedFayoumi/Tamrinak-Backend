using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.PaymentDtos
{
	public class AddPaymentDto
	{
		public int? BookingId { get; set; }
		public int? MembershipId { get; set; }

		[Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

		[Required]
		public PaymentMethod Method { get; set; }

		public string? TransactionId { get; set; }

	}
}
