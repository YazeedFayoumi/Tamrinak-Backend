namespace Tamrinak_API.DTO.PaymentDtos
{
    public class StripeIntentRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "jod";
    }
}
