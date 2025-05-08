namespace Tamrinak_API.DTO.MembershipOfferDtos
{
	public class MembershipOfferDto
	{
		public int OfferId { get; set; }
		public int FacilityId { get; set; }
		public int DurationInMonths { get; set; }
		public decimal Price { get; set; }
	}
}
