namespace Tamrinak_API.DTO.AdminDtos
{
    public class FacilityMembershipStatsDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int TotalMemberships { get; set; }
        public int ActiveMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int UniqueUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MembershipOfferBreakdownDto> OfferBreakdowns { get; set; }
        public List<AdminMembershipDto> Memberships { get; set; } 

    }
}
