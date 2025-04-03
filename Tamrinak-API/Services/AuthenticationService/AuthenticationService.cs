using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;
using Tamrinak_API.Repository.Repositories.GenericRepo;

namespace Tamrinak_API.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepo<User> _genericRepo;
        private readonly IGenericRepo<Role> _roleRepo;
        private readonly IGenericRepo<UserRole> _userRoleRepo;
        private readonly IConfiguration _config;
        public AuthenticationService(IGenericRepo<User> genericRepo, IConfiguration configuration, IGenericRepo<Role> roleRpo, IGenericRepo<UserRole> userRoleRepo)
        {
            _genericRepo = genericRepo;
            _config = configuration;
            _roleRepo = roleRpo;
            _userRoleRepo = userRoleRepo;

        }

        public async Task<LoginResponse> LoginAsync(UserLoginDto loginDto)
        {
            User user = await _genericRepo.GetByConditionAsync(e => e.Email == loginDto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");


            var token = await CreateTokenAsync(user);


            return token;

        }
        private async Task<LoginResponse> CreateTokenAsync(User user)
        {
            var userRoles = await _userRoleRepo.GetListByConditionAsync(ur => ur.UserId == user.UserId);
           
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var roles = await _roleRepo.GetListByConditionAsync(r => roleIds.Contains(r.RoleId));
            var roleNames = roles.Select(r => r.RoleName).ToList();

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),

            };

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:TokenKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(tokenDescriptor);

            return new LoginResponse
            {
                JwtToken = tokenString,
                Expiration = tokenDescriptor.ValidTo
            };


        }
    }
}
