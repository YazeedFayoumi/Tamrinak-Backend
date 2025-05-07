using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.PaymentDtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string? TransactionId { get; set; }
        public int? BookingId { get; set; }
        public int? MembershipId { get; set; }
        public bool IsConfirmed { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
