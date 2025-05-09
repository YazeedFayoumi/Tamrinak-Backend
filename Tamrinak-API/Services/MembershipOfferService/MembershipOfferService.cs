using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.MembershipOfferDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.MembershipOfferService
{
	public class MembershipOfferService : IMembershipOfferService
	{
		private readonly IGenericRepo<MembershipOffer> _offerRepo;
		private readonly IGenericRepo<Facility> _facilityRepo;

		public MembershipOfferService(IGenericRepo<MembershipOffer> offerRepo, IGenericRepo<Facility> facilityRepo)
		{
			_offerRepo = offerRepo;
			_facilityRepo = facilityRepo;
		}

		public async Task<MembershipOfferDto> AddOfferAsync(AddMembershipOfferDto dto)
		{
			var facility = await _facilityRepo.GetAsync(dto.FacilityId)
				?? throw new Exception("Facility not found");
			var exists = await _offerRepo.GetByConditionAsync(o =>
				 o.FacilityId == dto.FacilityId && o.DurationInMonths == dto.DurationInMonths);

			if (exists != null)
				throw new Exception("An offer with this duration already exists for this facility.");

			var offer = new MembershipOffer
			{
				FacilityId = dto.FacilityId,
				DurationInMonths = dto.DurationInMonths,
				Price = dto.Price
			};

			await _offerRepo.AddAsync(offer);
			await _offerRepo.SaveAsync();

			return new MembershipOfferDto
			{
				OfferId = offer.OfferId,
				FacilityId = offer.FacilityId,
				DurationInMonths = offer.DurationInMonths,
				Price = offer.Price
			};


		}

		public async Task<List<MembershipOfferDto>> GetOffersForFacilityAsync(int facilityId)
		{
			var offers = await _offerRepo.GetListByConditionAsync(o => o.FacilityId == facilityId);

			return offers.Select(o => new MembershipOfferDto
			{
				OfferId = o.OfferId,
				FacilityId = o.FacilityId,
				DurationInMonths = o.DurationInMonths,
				Price = o.Price
			}).ToList();
		}

		public async Task<bool> DeleteOfferAsync(int offerId)
		{
			var offer = await _offerRepo.GetAsync(offerId);
			if (offer == null) return false;

			await _offerRepo.DeleteAsync(offer);
			await _offerRepo.SaveAsync();
			return true;
		}

		public async Task<MembershipOfferDto> UpdateOfferAsync(int offerId, UpdateMembershipOfferDto dto)
		{
			var offer = await _offerRepo.GetAsync(offerId)
				?? throw new Exception("Offer not found");

			var exists = await _offerRepo.GetByConditionAsync(o =>
				o.FacilityId == offer.FacilityId &&
				o.DurationInMonths == dto.DurationInMonths &&
				o.OfferId != offerId);

			if (exists != null)
				throw new Exception("An offer with this duration already exists for this facility.");

			offer.DurationInMonths = dto.DurationInMonths;
			offer.Price = dto.Price;

			await _offerRepo.UpdateAsync(offer);
			await _offerRepo.SaveAsync();

			return new MembershipOfferDto
			{
				OfferId = offer.OfferId,
				FacilityId = offer.FacilityId,
				DurationInMonths = offer.DurationInMonths,
				Price = offer.Price
			};
		}

	}
}
