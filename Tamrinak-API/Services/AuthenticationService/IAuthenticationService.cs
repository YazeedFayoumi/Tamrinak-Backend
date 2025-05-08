using System.Threading.Tasks;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.UserAuthDtos;

namespace Tamrinak_API.Services.AuthenticationService
{
	public interface IAuthenticationService
	{
		Task<LoginResponse> LoginAsync(UserLoginDto loginDto);
		Task SendConfirmationEmailAsync(string email);

		Task<bool> ConfirmEmailAsync(string token);

		Task SendResetPasswordEmailAsync(string email);

		Task<bool> ResetPasswordAsync(string token, string newPassword);
	}
}
