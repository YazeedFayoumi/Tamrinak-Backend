namespace Tamrinak_API.DTO.MembershipDtos
{
    public class MembershipDto
    {
        public int MembershipId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal MonthlyFee { get; set; }
        public bool IsActive { get; set; }
    }
}
