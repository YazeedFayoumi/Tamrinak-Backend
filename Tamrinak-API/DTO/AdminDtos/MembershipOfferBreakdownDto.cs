namespace Tamrinak_API.DTO.AdminDtos
{
    public class MembershipOfferBreakdownDto
    {
        public int? OfferId { get; set; } // null = no offer
        public string OfferDuration { get; set; } // e.g., "3-Month Offer"
        public int Count { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
