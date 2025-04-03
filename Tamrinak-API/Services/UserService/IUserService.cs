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
    }
}
