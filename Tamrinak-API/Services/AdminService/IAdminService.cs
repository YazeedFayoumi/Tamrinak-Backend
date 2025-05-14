using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.UserAuthDtos;

namespace Tamrinak_API.Services.AdminService
{
    public interface IAdminService 
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();
        Task<AdminUserDto> GetUserByIdAsync(int userId);
        //Task<bool> ChangeUserRoleAsync(int userId, string roleName);
        Task<List<AdminUserDto>> GetVenueOwnershipRequestsAsync();
        Task<bool> ApproveVenueOwnershipRequestAsync(int userId);
        Task<bool> RejectVenueOwnershipRequestAsync(int userId);
        Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);
        Task<bool> DeleteUserAsync(int userId);
        Task AddRoleToUserAsync(int userId, string roleName);
        Task RemoveRoleToUserAsync(int userId, string roleName);

    }
}
