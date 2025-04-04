using System.Data;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;
using Tamrinak_API.Repository.Repositories.GenericRepo;

namespace Tamrinak_API.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IGenericRepo<User> _genericRepo;
        private readonly IGenericRepo<Role> _roleRepo;
        private readonly IGenericRepo<UserRole> _userRoleRepo;
        public UserService(IGenericRepo<User> genericRepo, IGenericRepo<Role> roleRepo, IGenericRepo<UserRole> userRoleRepo)
        {
            _genericRepo = genericRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
        }


        public async Task<User> CreateUserAsync(UserRegisterDto user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            User newUser = new User
            {
                Email = user.Email,
                PasswordHash = hashedPassword,
                Name = user.Name
            };

            var createdUser = await _genericRepo.CreateAsync(newUser);
            return createdUser; 
        }

        public async Task AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            var user = await _genericRepo.GetByConditionAsync(u => u.Email == assignRoleDto.UserEmail);
            if (user == null) throw new Exception("User not found");

            List<Role> roles = await _roleRepo.GetListByConditionAsync(r => assignRoleDto.RoleNames.Contains(r.RoleName))??throw new Exception("Role not found");

            /* var existingUserRole = await _userRoleRepo.GetByConditionAsync(ur => ur.UserId == user.UserId && ur.RoleId == roles.RoleId);
             if (existingUserRole != null) throw new Exception("User already has this role");*/
            foreach (var role in roles)
            {
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                };
                await _userRoleRepo.AddAsync(userRole);
            }
            await _genericRepo.SaveAsync();
         

            await _genericRepo.UpdateAsync(user);
        }
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var roleIds = (await _userRoleRepo.GetListByConditionAsync(ur => ur.UserId == userId))
                          .Select(ur => ur.RoleId)
                          .ToList();

            return (await _roleRepo.GetListByConditionAsync(r => roleIds.Contains(r.RoleId)))
                   .Select(r => r.RoleName)
                   .ToList();
        }

        public async Task<List<string>> GetUserRolesAsync(string email)
        {
            var user = (await _genericRepo.GetByConditionAsync(e => e.Email == email));
            var roleIds = (await _userRoleRepo.GetListByConditionAsync(ur => ur.UserId == user.UserId))
                          .Select(ur => ur.RoleId)
                          .ToList();

            return (await _roleRepo.GetListByConditionAsync(r => roleIds.Contains(r.RoleId)))
                   .Select(r => r.RoleName)
                   .ToList();
        }
    }
}
