using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DTO;
using Tamrinak_API.Services.AuthenticationService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult> LoginUser([FromBody] UserLoginDto loginUser)
        {
            LoginResponse response = await _authenticationService.LoginAsync(loginUser);
            return Ok(response);
        }
    }
}
