using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;

namespace Tamrinak_API.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(UserLoginDto loginDto);

    }
}
