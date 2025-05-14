using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepo<User> _userRepo;
        private readonly IGenericRepo<UserRole> _userRoleRepo;
        private readonly IGenericRepo<Role> _roleRepo;

        public AdminService(IGenericRepo<User> userRepo, IGenericRepo<UserRole> userRoleRepo, IGenericRepo<Role> roleRepo)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _roleRepo = roleRepo;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetListByConditionIncludeAsync(
     predicate: u => true, // or any real filter if needed
     include: query => query
         .Include(u => u.UserRoles)
         .ThenInclude(ur => ur.Role)
 );


            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = u.HasVenueOwnershipRequest,
                VenueRequestDate = u.VenueRequestDate
            }).ToList();
        }

        public async Task<AdminUserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepo.GetByConditionIncludeAsync(
                u => u.UserId == userId,
                include: u => u.Include(x => x.UserRoles).ThenInclude(ur => ur.Role));

            if (user == null)
                throw new Exception("User not found");

            return new AdminUserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = user.HasVenueOwnershipRequest,
                VenueRequestDate = user.VenueRequestDate
            };
        }

        public async Task AddRoleToUserAsync(int userId, string roleName)
        {
            var user = await _userRepo.GetByConditionAsync(u => u.UserId == userId);
            if (user == null) throw new Exception("User not found");

            var role = await _roleRepo.GetByConditionAsync(r => r.RoleName == roleName);
            if (role == null) throw new Exception("Role not found");

            var exists = await _userRoleRepo.GetByConditionAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);
            if (exists != null) throw new Exception("User already has this role");

            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = role.RoleId
            });
            await _userRoleRepo.SaveAsync();
        }

        public async Task RemoveRoleToUserAsync(int userId, string roleName)
        {
            var role = await _roleRepo.GetByConditionAsync(r => r.RoleName == roleName);
            if (role == null) throw new Exception("Role not found");

            var userRole = await _userRoleRepo.GetByConditionAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);
            if (userRole == null) throw new Exception("Role not assigned to user");

            await _userRoleRepo.DeleteAsync(userRole);
            await _userRoleRepo.SaveAsync();
        }

        public async Task<List<AdminUserDto>> GetVenueOwnershipRequestsAsync()
        {
            var users = await _userRepo.GetListByConditionAsync(u => u.HasVenueOwnershipRequest == true);
            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = u.HasVenueOwnershipRequest,
                VenueRequestDate = u.VenueRequestDate
            }).ToList();
        }

        public async Task<bool> ApproveVenueOwnershipRequestAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");

            user.HasVenueOwnershipRequest = false;
            user.VenueRequestDate = null;

            var venueManagerRole = await _roleRepo.GetByConditionAsync(r => r.RoleName == "VenueManager");
            if (venueManagerRole == null)
                throw new Exception("VenueManager role not found");

            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = venueManagerRole.RoleId
            });

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> RejectVenueOwnershipRequestAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");

            user.HasVenueOwnershipRequest = false;
            user.VenueRequestDate = null;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");
            user.IsActive = isActive;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");
            await _userRepo.DeleteAsync(user);
            await _userRepo.SaveAsync();
            return true;
        }
    }
}
