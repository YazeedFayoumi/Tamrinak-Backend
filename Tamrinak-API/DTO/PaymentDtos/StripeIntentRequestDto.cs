namespace Tamrinak_API.DTO.PaymentDtos
{
    public class StripeIntentRequestDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "jod";
        public int? BookingId { get; set; }
        public int? MembershipId { get; set; }
    }
}
