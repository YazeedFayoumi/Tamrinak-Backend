namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminPaymentDto
    {
        public int PaymentId { get; set; }
        public int? BookingId { get; set; }
        public int? MembershipId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string? TransactionId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRefunded { get; set; }
    }
}
