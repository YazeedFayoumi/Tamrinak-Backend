using Tamrinak_API.DTO.MembershipDtos;

namespace Tamrinak_API.Services.MembershipService
{
	public interface IMembershipService
	{
		Task<MembershipDto> AddMembershipAsync(AddMembershipDto dto, string userEmail);
		Task<List<MembershipDto>> GetUserMembershipsAsync(string userEmail);
		Task<MembershipDto> GetMembershipByIdAsync(int id);
		Task CancelMembershipAsync(int id, string userEmail);
		Task DeleteMembershipAsync(int id, string userEmail);
	}
}
