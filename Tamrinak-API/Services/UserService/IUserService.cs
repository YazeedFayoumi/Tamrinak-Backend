using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;

namespace Tamrinak_API.Services.UserService
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserRegisterDto user);  
        Task AssignRoleAsync(AssignRoleDto assignRole);
        Task<List<string>> GetUserRolesAsync(int id);
        Task<List<string>> GetUserRolesAsync(string email);
        Task<User> GetUserAsync(int id);
        Task<List<UserListInfo>> GetAllUsersAsync();
        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(User user);
        Task<bool> CanAddUserImageAsync(int userId);
    }
}
