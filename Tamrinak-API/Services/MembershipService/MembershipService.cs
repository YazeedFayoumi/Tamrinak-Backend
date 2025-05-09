using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.MembershipDtos;
using Tamrinak_API.Repository.GenericRepo;
using Tamrinak_API.Services.EmailService;

namespace Tamrinak_API.Services.MembershipService
{
	public class MembershipService : IMembershipService
	{
		private readonly IGenericRepo<Membership> _membershipRepo;
		private readonly IGenericRepo<Facility> _facilityRepo;
		private readonly IGenericRepo<User> _userRepo;
        private readonly IGenericRepo<MembershipOffer> _offerRepo;
        private readonly IEmailService _emailService;
		
		public MembershipService(IGenericRepo<Membership> membershipRepo, IGenericRepo<Facility> facilityRepo, IGenericRepo<User> userRepo, IGenericRepo<MembershipOffer> offerRepo , IEmailService emailService)
		{
			_membershipRepo = membershipRepo;
			_facilityRepo = facilityRepo;
			_userRepo = userRepo;
			_emailService = emailService;
			_offerRepo = offerRepo;
		}

		public async Task<MembershipDto> AddMembershipAsync(AddMembershipDto dto, string userEmail)
		{
            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
              ?? throw new Exception("User not found");

            if (user.IsEmailConfirmed != true)
                throw new Exception("Please confirm your email.");

            var facility = await _facilityRepo.GetAsync(dto.FacilityId)
                           ?? throw new Exception("Facility not found");

            decimal finalMonthlyFee;
            int durationInMonths;
			decimal? totalOfferPaid;
            if (dto.OfferId.HasValue)
            {
                var offer = await _offerRepo.GetAsync(dto.OfferId.Value)
                            ?? throw new Exception("Offer not found");

                if (offer.FacilityId != dto.FacilityId)
                    throw new Exception("Offer does not belong to the selected facility.");

                finalMonthlyFee = Math.Round(offer.Price / offer.DurationInMonths, 2);
                durationInMonths = offer.DurationInMonths;
                totalOfferPaid = offer.Price;
            }
            else
            {
                if (facility.PricePerMonth == null)
                    throw new Exception("No pricing available for this facility.");

                finalMonthlyFee = facility.PricePerMonth.Value;
                durationInMonths = 1; // default
				totalOfferPaid = null;
            }

            var now = DateTime.UtcNow.Date;

            var membership = new Membership
            {
                UserId = user.UserId,
                FacilityId = facility.FacilityId,
                StartDate = now,
                ExpirationDate = now.AddMonths(durationInMonths),
                MonthlyFee = finalMonthlyFee,
                TotalOfferPaid = totalOfferPaid,
                IsActive = true
            };

            await _membershipRepo.AddAsync(membership);
            await _membershipRepo.SaveAsync();

            return new MembershipDto
            {
                MembershipId = membership.MembershipId,
                FacilityId = membership.FacilityId,
                FacilityName = facility.Name,
                StartDate = membership.StartDate,
                ExpirationDate = membership.ExpirationDate,
                MonthlyFee = membership.MonthlyFee,
				TotalOfferPaid = totalOfferPaid,
                IsActive = membership.IsActive
            };
        }

		public async Task<List<MembershipDto>> GetUserMembershipsAsync(string userEmail)

		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
					   ?? throw new Exception("User not found");

			var memberships = await _membershipRepo.GetListByConditionAsync(m => m.UserId == user.UserId);

			var facilities = (await _facilityRepo.GetAllAsync()).ToDictionary(f => f.FacilityId, f => f.Name);

			return memberships.Select(m => new MembershipDto
			{
				MembershipId = m.MembershipId,
				FacilityId = m.FacilityId,
				FacilityName = facilities.ContainsKey(m.FacilityId) ? facilities[m.FacilityId] : "Unknown",
				StartDate = m.StartDate,
				ExpirationDate = m.ExpirationDate,
				MonthlyFee = m.MonthlyFee,
				TotalOfferPaid = m.TotalOfferPaid,
				IsActive = m.IsActive
			}).ToList();
		}

		public async Task<MembershipDto> GetMembershipByIdAsync(int id)
		{
			var m = await _membershipRepo.GetAsync(id)
					?? throw new Exception("Membership not found");

			var facility = await _facilityRepo.GetAsync(m.FacilityId)
						  ?? throw new Exception("Facility not found");

			return new MembershipDto
			{
				MembershipId = m.MembershipId,
				FacilityId = m.FacilityId,
				FacilityName = facility.Name,
				StartDate = m.StartDate,
				ExpirationDate = m.ExpirationDate,
				MonthlyFee = m.MonthlyFee,
				TotalOfferPaid = m.TotalOfferPaid,
				IsActive = m.IsActive
			};
		}

		public async Task CancelMembershipAsync(int id, string userEmail)
		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
					   ?? throw new Exception("User not found");

			var membership = await _membershipRepo.GetAsync(id)
							  ?? throw new Exception("Membership not found");

			if (membership.UserId != user.UserId)
				throw new Exception("Unauthorized to cancel this membership.");

			membership.IsActive = false;
			await _membershipRepo.UpdateAsync(membership);
			await _membershipRepo.SaveAsync();
		}

		public async Task DeleteMembershipAsync(int id, string userEmail)
		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
					   ?? throw new Exception("User not found");

			var membership = await _membershipRepo.GetAsync(id)
							  ?? throw new Exception("Membership not found");

			if (membership.UserId != user.UserId)
				throw new Exception("Unauthorized to delete this membership.");

			await _membershipRepo.DeleteAsync(membership);
			await _membershipRepo.SaveAsync();
		}

        public async Task<int> DeactivateExpiredMembershipsAsync()
        {
            var now = DateTime.UtcNow.Date;

            var expiredMemberships = await _membershipRepo.GetListByConditionAsync(
                m => m.IsActive && m.ExpirationDate < now
            );

            foreach (var membership in expiredMemberships)
            {
                membership.IsActive = false;
                await _membershipRepo.UpdateAsync(membership);
            }

            await _membershipRepo.SaveAsync();

            return expiredMemberships.Count;
        }

        public async Task<int> SendMembershipExpiryRemindersAsync()
        {
            var targetDate = DateTime.UtcNow.Date.AddDays(29);

            var memberships = await _membershipRepo.GetListByConditionIncludeAsync(
                m => m.ExpirationDate.Date == targetDate && m.IsActive,
                include: q => q.Include(m => m.User).Include(m => m.Facility)
            );

            foreach (var m in memberships)
            {
                await _emailService.SendMembershipExpiryReminderAsync(m.User.Email, m.User.Name, m.Facility.Name, m.ExpirationDate);
            }

            return memberships.Count;
        }
    }
}
