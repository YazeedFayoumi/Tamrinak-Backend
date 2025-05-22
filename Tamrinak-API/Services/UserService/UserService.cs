using Microsoft.EntityFrameworkCore;
using System.Data;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.UserService
{
	public class UserService : IUserService
	{
		private readonly IGenericRepo<User> _genericRepo;
		private readonly IGenericRepo<Role> _roleRepo;
		private readonly IGenericRepo<UserRole> _userRoleRepo;
		private readonly IGenericRepo<Field> _fieldRepo;	
		private readonly IGenericRepo<Facility> _facilityRepo;

		public UserService(IGenericRepo<User> genericRepo, IGenericRepo<Role> roleRepo, IGenericRepo<UserRole> userRoleRepo,
  IGenericRepo<Field> fieldRepo, IGenericRepo<Facility> facilityRepo)
        {
            _genericRepo = genericRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _fieldRepo = fieldRepo;
            _facilityRepo = facilityRepo;
        }


        public async Task<UserDto> CreateUserAsync(UserRegisterDto user)
		{
			if (await _genericRepo.ExistsAsync(e => e.Email == user.Email))
			{
				throw new InvalidOperationException($"Email {user.Email} is already registered.");
			}
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
			var role = await _roleRepo.GetByConditionAsync(r => r.RoleName == "User");

			User newUser = new User
			{
				Email = user.Email,
				PasswordHash = hashedPassword,
				Name = user.Name,
				UserRoles = new List<UserRole>()

			};
			var userRole = new UserRole
			{
				RoleId = role.RoleId,
				Role = role,
				User = newUser,
				AssignedDate = DateTime.Now
			};
			newUser.UserRoles.Add(userRole);

			var createdUser = await _genericRepo.CreateAsync(newUser);
			return new UserDto
			{
				Id = newUser.UserId,
				Email = newUser.Email,
				Name = newUser.Name,
                ProfileImageBase64 = newUser.ProfileImageBase64,
				Roles = newUser.UserRoles.Select(ur => ur.Role.RoleName).ToList()
			};
		}

		public async Task AssignRoleAsync(AssignRoleDto assignRoleDto)
		{
			var user = await _genericRepo.GetByConditionAsync(u => u.Email == assignRoleDto.UserEmail);
			if (user == null) throw new Exception("User not found");

			List<Role> roles = await _roleRepo.GetListByConditionAsync(r => assignRoleDto.RoleNames.Contains(r.RoleName)) ?? throw new Exception("Role not found");

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

		public async Task DeleteUserAsync(int id)
		{
			var user = await _genericRepo.GetAsync(id);

			await _genericRepo.DeleteAsync(user);
			await _genericRepo.SaveAsync();
		}
		public async Task<User> GetUserAsync(int id)
		{
			var user = await _genericRepo.GetAsync(id);
			return user;
		}
		public async Task<UserDto> GetUserDtoAsync(int id)
		{
			var user = await _genericRepo.GetAsync(id);
			var userRoles = await _userRoleRepo.GetListByConditionIncludeAsync(
				ur => ur.UserId == id,
				q => q.Include(ur => ur.Role)
			);
			var userDto = new UserDto
			{
				Id = user.UserId,
				Name = user.Name,
				Email = user.Email,
				ProfileImageBase64 = user.ProfileImageBase64,
				Roles = userRoles.Select(ur => ur.Role.RoleName).ToList(),
			};
			return userDto;
		}

		public async Task<List<UserListInfo>> GetAllUsersAsync()
		{
			var users = await _genericRepo.GetAllAsync();

			return users.Select(u => new UserListInfo
			{
				UserId = u.UserId,
				Email = u.Email,
				Name = u.Name
			}).ToList();
		}
		public async Task UpdateUserAsync(User user)
		{
			await _genericRepo.UpdateAsync(user);
			await _genericRepo.SaveAsync();
		}

		public async Task<bool> CanAddUserImageAsync(int userId)
		{
			var user = await _genericRepo.GetByConditionAsync(u => u.UserId == userId);
			return string.IsNullOrEmpty(user.ProfileImageBase64); // true if user has no image yet
		}

		public async Task<ProfileDto> GetUserProfileAsync(string email)
		{
			var user = await _genericRepo.GetByConditionIncludeAsync(
				u => u.Email == email,
				include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
			);

			if (user == null)
				throw new KeyNotFoundException("User not found");

			return new ProfileDto
			{
				Id = user.UserId,
				Name = user.Name,
				Email = user.Email,
				ProfileImageBase64 = user.ProfileImageBase64,
				Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
			};
		}

        public async Task RequestVenueOwnershipAsync(string email, VenueOwnershipRequestDto reqDto)
        {
            var user = await _genericRepo.GetByConditionAsync(u => u.Email == email) ?? throw new Exception("User not found");
            if (reqDto.VenueType != "Facility" && reqDto.VenueType != "Field")
                throw new Exception("Invalid venue type.");

            if (user.HasVenueOwnershipRequest)
                throw new Exception("You already have a pending venue ownership request.");

            if (reqDto.VenueType == "Facility")
            {
                var facility = await _facilityRepo.GetAsync(reqDto.VenueId)
                    ?? throw new Exception("Facility not found.");
                if (facility.OwnerId != null)
                    throw new Exception("This facility is already managed.");
            }
            else 
            {
                var field = await _fieldRepo.GetAsync(reqDto.VenueId)
                    ?? throw new Exception("Field not found.");
                if (field.OwnerId != null)
                    throw new Exception("This field is already managed.");
            }

            user.HasVenueOwnershipRequest = true;
            user.VenueRequestDate = DateTime.UtcNow;
            user.RequestedVenueId = reqDto.VenueId;
            user.RequestedVenueType = reqDto.VenueType;

            await _genericRepo.UpdateAsync(user);
            await _genericRepo.SaveAsync();
        }

        public async Task RequestVenueManagerRoleAsync(int userId)
        {
            var user = await _genericRepo.GetAsync(userId) ?? throw new Exception("User not found");

            if (user.HasVenueOwnershipRequest)
                throw new Exception("You already have a pending request.");

            user.HasVenueOwnershipRequest = true;
            user.VenueRequestDate = DateTime.UtcNow;
            user.RequestedVenueId = null;
            user.RequestedVenueType = null;

            await _genericRepo.UpdateAsync(user);
            await _genericRepo.SaveAsync();
        }



    }
}
