using Tamrinak_API.DTO.MembershipOfferDtos;

namespace Tamrinak_API.Services.MembershipOfferService
{
	public interface IMembershipOfferService
	{
		Task<List<MembershipOfferDto>> GetOffersForFacilityAsync(int facilityId);
		Task<MembershipOfferDto> AddOfferAsync(AddMembershipOfferDto dto);
		Task<bool> DeleteOfferAsync(int offerId);
		Task<MembershipOfferDto> UpdateOfferAsync(int offerId, UpdateMembershipOfferDto dto);
	}
}
