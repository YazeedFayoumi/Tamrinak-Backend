namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminMembershipDto
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }  
        public string? UserEmail { get; set; }
        public string FacilityName { get; set; }     // From Facility
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MonthlyFee { get; set; }
        public decimal? TotalOfferPaid { get; set; }
    }
}
