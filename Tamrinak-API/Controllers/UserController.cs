using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register"), Authorize]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            var createdUser = await _userService.CreateUserAsync(registerDto);
            return Ok(createdUser);
        }
        [HttpPatch("AddRole")]
        public async Task<ActionResult> AddRoleToCustomer([FromBody] AssignRoleDto model)
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
    }
}
