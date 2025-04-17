using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO;
using Tamrinak_API.Services.AuthenticationService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authService = authenticationService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult> LoginUser([FromBody] UserLoginDto loginUser)
        {
            LoginResponse response = await _authService.LoginAsync(loginUser);
            return Ok(response);
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt"); 
            return Ok("Logged out successfully.");
        }
       /* [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var newTokens = await _authService.RefreshTokenAsync(refreshToken);
            if (newTokens == null)
                return Unauthorized("Invalid or expired refresh token.");

            return Ok(newTokens); // Return new access + refresh tokens
        }*/
        [Authorize]
        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("No email found in token");

                await _authService.SendConfirmationEmailAsync(email);
                return Ok("Confirmation email sent to: "+ email);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
            
        }

  
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                var success = await _authService.ConfirmEmailAsync(token);
                if (!success)
                    return BadRequest("Invalid or expired token");

                return Ok("Email confirmed successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
         
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                await _authService.SendResetPasswordEmailAsync(email);
                return Ok("Password reset email sent to: " + email);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
                if (!result)
                    return BadRequest("Invalid or expired token");

                return Ok("Password reset successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }
}
