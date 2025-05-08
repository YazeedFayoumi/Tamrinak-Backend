using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.MembershipDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.MembershipService
{
	public class MembershipService : IMembershipService
	{
		private readonly IGenericRepo<Membership> _membershipRepo;
		private readonly IGenericRepo<Facility> _facilityRepo;
		private readonly IGenericRepo<User> _userRepo;
		public MembershipService(IGenericRepo<Membership> membershipRepo, IGenericRepo<Facility> facilityRepo, IGenericRepo<User> userRepo)
		{
			_membershipRepo = membershipRepo;
			_facilityRepo = facilityRepo;
			_userRepo = userRepo;
		}

		public async Task<MembershipDto> AddMembershipAsync(AddMembershipDto dto, string userEmail)
		{
			var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
					   ?? throw new Exception("User not found or not confirmed");

			if (user.IsEmailConfirmed != true)
				throw new Exception("Please confirm your email before subscribing.");

			var facility = await _facilityRepo.GetAsync(dto.FacilityId)
						  ?? throw new Exception("Facility not found");

			var startDate = DateTime.UtcNow.Date;
			DateTime expirationDate;
			decimal monthlyFee;

			if (facility.OfferDurationInMonths.HasValue && facility.OfferDurationInMonths.Value > 0 && facility.OfferPrice.HasValue)
			{
				monthlyFee = facility.OfferPrice.Value / facility.OfferDurationInMonths.Value;
				expirationDate = startDate.AddMonths(facility.OfferDurationInMonths.Value);
			}
			else
			{
				if (facility.PricePerMonth is null)
					throw new InvalidOperationException("Facility does not have a valid pricing configuration.");

				monthlyFee = facility.PricePerMonth.Value;
				expirationDate = startDate.AddMonths(1); // standard monthly membership
			}

			var membership = new Membership
			{
				UserId = user.UserId,
				FacilityId = facility.FacilityId,
				StartDate = startDate,
				ExpirationDate = expirationDate,
				MonthlyFee = monthlyFee,
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

	}
}
