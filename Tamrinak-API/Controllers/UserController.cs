using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Helpers;
using Tamrinak_API.Services.AuthenticationService;
using Tamrinak_API.Services.ImageService;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IAuthenticationService _authService;
		private readonly IImageService _imageService;
		public UserController(IUserService userService, IImageService imageService, IAuthenticationService authenticationService)
		{
			_userService = userService;
			_authService = authenticationService;
			_imageService = imageService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(UserRegisterDto registerDto)
		{
			try
			{
				UserDto createdUser = await _userService.CreateUserAsync(registerDto);
				return Ok(createdUser);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
			}

		}

		[HttpGet("UserRoles/{id}"), Authorize]
		public async Task<ActionResult> GetUserRoles(int id)
		{
			var roles = await _userService.GetUserRolesAsync(id);
			return Ok(roles);
		}

		//[HttpGet("UserRolesByEmail/{email}"), AuthorizeRole(Roles = "Admin, SuperAdmin")]
		//public async Task<ActionResult> GetUserRoles([FromBody] string email)
		//{
		//	var roles = await _userService.GetUserRolesAsync(email);
		//	return Ok(roles);
		//}
		/*[HttpGet("BasicInfoUserList")]
		public async Task<IActionResult> GetUsersBasicInfo()
		{
			var users = await _userService.GetAllUsersAsync();
			return Ok(users);
		}
*/

		[HttpGet("{id}")]
		//[Authorize]
		public async Task<ActionResult<UserDto>> GetUserById(int id)
		{
			try
			{
				var userDto = await _userService.GetUserDtoAsync(id);

				if (userDto == null)
				{
					return NotFound($"User with ID {id} not found");
				}
				return Ok(userDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		//[Authorize] 
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				await _userService.DeleteUserAsync(id);
				return Ok(new { message = $"User with ID {id} deleted successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpDelete("delete-profile-picture")]
		public async Task<IActionResult> DeleteProfilePicture(int userId)
		{
			try
			{
				var user = await _userService.GetUserAsync(userId);
				if (user == null)
					return NotFound("User not found.");

				if (string.IsNullOrEmpty(user.ProfileImageBase64))
					return BadRequest("No profile image to delete.");

				//var result = await _imageService.DeleteImageAsync(user.ProfileImageBase64);
				//if (!result)
				return StatusCode(500, "Error deleting the image.");

				user.ProfileImageBase64 = null;
				await _userService.UpdateUserAsync(user);

				return Ok("Profile image deleted successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}

		}

		[HttpPatch("profile-picture")]
		public async Task<IActionResult> UpdateProfilePicture(IFormFile file, int userId)
		{
			if (file == null || file.Length == 0)
				return BadRequest("No file uploaded.");

			try
			{
				var user = await _userService.GetUserAsync(userId);
				if (user == null)
					return NotFound("User not found");

				// Delete existing image if one exists and replacement is allowed
				if (!string.IsNullOrEmpty(user.ProfileImageBase64))
				{
					user.ProfileImageBase64 = null;
					await _userService.UpdateUserAsync(user);
				}

				var folderName = "users";//?
				var imageBase64 = await _imageService.UploadImageAsync(file, folderName);

				user.ProfileImageBase64 = imageBase64;
				await _userService.UpdateUserAsync(user);

				return Ok(new
				{
					message = "Profile picture updated successfully",
					imageUrl = imageBase64
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("profile-picture")]
		public async Task<IActionResult> GetProfilePictureFile(int userId)
		{
			var user = await _userService.GetUserAsync(userId);
			if (user == null || string.IsNullOrEmpty(user.ProfileImageBase64))
				return NotFound("Profile image not found.");


			// Return Base64 image as data URI

			/*            var imageFileName = Path.GetFileName(user.ProfileImageBase64);
						var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users", imageFileName);*/

			/* if (!System.IO.File.Exists(imagePath))
                 return NotFound("Image file not found on server.");*/
			try
			{
				var base64Image = user.ProfileImageBase64;
				var dataUri = $"data:image/jpeg;base64,{base64Image}";
				var contentType = _imageService.GetContentType(base64Image);

				return Ok(new
				{
					Base64Image = $"data:image/jpeg;base64,{user.ProfileImageBase64}"
				});
			}
			catch (UnauthorizedAccessException)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Access denied to image file.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
			}
		}

		[HttpGet("profile")]
		[Authorize]
		public async Task<IActionResult> GetUserProfile()
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(email))
					return Unauthorized();

				var profile = await _userService.GetUserProfileAsync(email);
				return Ok(profile);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[Authorize]
		[HttpPost("request-venue-ownership-existing-venue")]
		public async Task<IActionResult> RequestVenueOwnership([FromBody] VenueOwnershipRequestDto reqDto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value; // Replace with your actual claim extraction
				await _userService.RequestVenueOwnershipAsync(email, reqDto);
				return Ok("Venue ownership request submitted.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("request-venue-manager")]
		[Authorize]
		public async Task<IActionResult> RequestVenueManagerRole()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				await _userService.RequestVenueManagerRoleAsync(userId);
				return Ok("Venue manager role request submitted.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}


}
