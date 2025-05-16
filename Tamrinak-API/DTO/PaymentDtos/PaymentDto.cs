using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.PaymentDtos
{
	public class PaymentDto
	{
        public int PaymentId { get; set; }
        public string ForType { get; set; } // "Booking" or "Membership"
        public int ReferenceId { get; set; } // BookingId or MembershipId
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRefunded { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
