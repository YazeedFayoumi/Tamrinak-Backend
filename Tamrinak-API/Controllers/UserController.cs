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
                var createdUser = await _userService.CreateUserAsync(registerDto);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            
        }

        [HttpPatch("AddRole"), AuthorizeRole(Roles ="SuperAdmin,Admin")]
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

        [HttpPost("upload-profile-picture")]
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
    }
}
