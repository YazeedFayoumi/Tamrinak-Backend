using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;
using Tamrinak_API.Services.EmailService;

namespace Tamrinak_API.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepo<User> _genericRepo;
        private readonly IGenericRepo<Role> _roleRepo;
        private readonly IGenericRepo<UserRole> _userRoleRepo;
        private readonly IEmailService _emailService;

        private readonly IConfiguration _config;
        public AuthenticationService(IGenericRepo<User> genericRepo, IConfiguration configuration, IGenericRepo<Role> roleRpo, IGenericRepo<UserRole> userRoleRepo, IEmailService emailService)
        {
            _genericRepo = genericRepo;
            _config = configuration;
            _roleRepo = roleRpo;
            _userRoleRepo = userRoleRepo;
            _emailService = emailService;
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
        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:TokenKey"]);

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var email = claimsPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
                if (email != null)
                {
                    var user = await _genericRepo.GetByConditionAsync(u => u.Email == email);
                    if (user == null) return false;

                    user.IsEmailConfirmed = true;
                    await _genericRepo.UpdateAsync(user);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false; // Token is invalid or expired
            }
        }
        private string GenerateEmailToken(string email)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:TokenKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task SendConfirmationEmailAsync(string userEmail)
        {
            var user = await _genericRepo.GetByConditionAsync(u => u.Email == userEmail);
            if (user == null)
                throw new Exception("User not found");

            var token = GenerateEmailToken(user.Email);
            var confirmUrl = $"{_config["ClientApp:BaseUrl"]}/confirm-email?token={token}";

            await _emailService.SendConfirmationEmailAsync(user.Email, confirmUrl);
        }

        public async Task SendResetPasswordEmailAsync(string email)
        {
            var user = await _genericRepo.GetByConditionAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found");

            var token = GenerateEmailToken(user.Email);
            var resetUrl = $"{_config["ClientApp:BaseUrl"]}/reset-password?token={token}";

            await _emailService.SendPasswordResetEmailAsync(email, resetUrl);
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:TokenKey"]);

            try
            {
                var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _);

                var email = claimsPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null) return false;

                var user = await _genericRepo.GetByConditionAsync(u => u.Email == email);
                if (user == null) return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _genericRepo.UpdateAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
