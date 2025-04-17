using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;
using Tamrinak_API.Services.ImageService;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        public UserController(IUserService userService, IImageService imageService)
        {
            _userService = userService;
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

        [HttpPatch("AddRole"), AuthorizeRole(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> AddRoleToUser([FromBody] AssignRoleDto model)
        {
            try
            {
                await _userService.AssignRoleAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("GetUserRoles"), Authorize]
        public async Task<ActionResult> GetUserRoles([FromBody] int id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }

        [HttpPost("GetUserRolesEmail"), AuthorizeRole(Roles = "Admin")]
        public async Task<ActionResult> GetUserRoles([FromBody] string email)
        {
            var roles = await _userService.GetUserRolesAsync(email);
            return Ok(roles);
        }
        [HttpGet("BasicInfoUserList")]
        public async Task<IActionResult> GetUsersBasicInfo()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
            return Ok(user);
        }

        [HttpPatch("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var folderName = "users";

            try
            {
                var user = await _userService.GetUserAsync(userId);

                if (user == null)
                    return NotFound("User not found.");

                if (!await _userService.CanAddUserImageAsync(userId))
                    return BadRequest("User already has a profile image.");

                var imageUrl = await _imageService.UploadImageAsync(file, folderName);

                user.ProfileImageUrl = imageUrl;
                await _userService.UpdateUserAsync(user);

                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }
        [HttpDelete("delete-profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture(int userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found.");

                if (string.IsNullOrEmpty(user.ProfileImageUrl))
                    return BadRequest("No profile image to delete.");

                var result = await _imageService.DeleteImageAsync(user.ProfileImageUrl);
                if (!result)
                    return StatusCode(500, "Error deleting the image.");

                user.ProfileImageUrl = null;
                await _userService.UpdateUserAsync(user);

                return Ok("Profile image deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPatch("change-profile-picture")]
        public async Task<IActionResult> ChangeProfilePicture(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            try
            {
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                if (await _userService.CanAddUserImageAsync(userId))
                {
                    await _imageService.DeleteImageAsync(user.ProfileImageUrl);
                }

                var folderName = "users";
                var imageUrl = await _imageService.UploadImageAsync(file, folderName);

                user.ProfileImageUrl = imageUrl;
                await _userService.UpdateUserAsync(user);

                return Ok(new { message = "New profile picture saved", imageUrl = imageUrl });
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }
        
        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetProfilePictureFile(int userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.ProfileImageUrl))
                return NotFound("Profile image not found.");

            var imageFileName = Path.GetFileName(user.ProfileImageUrl);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users", imageFileName);

            /* if (!System.IO.File.Exists(imagePath))
                 return NotFound("Image file not found on server.");*/
            try
            {
                var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var contentType = GetContentType(imagePath);
                return File(stream, contentType);
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
        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }


}
