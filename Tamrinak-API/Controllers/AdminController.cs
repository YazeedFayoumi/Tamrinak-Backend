using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.Services.AdminService;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
   /// <summary>
   /// /////////[Authorize(Roles = "SuperAdmin")]
   /// </summary>
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            return user != null ? Ok(user) : NotFound("User not found");
        }

        [HttpPost("users/{id}/add-role")]
        public async Task<IActionResult> AddRoleToUser(int id, [FromBody] string roleName)
        {
            await _adminService.AddRoleToUserAsync(id, roleName);
            return Ok("Role added");
        }

        [HttpPost("users/{id}/remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser(int id, [FromBody] string roleName)
        {
            await _adminService.RemoveRoleToUserAsync(id, roleName);
            return Ok("Role removed");
        }

        [HttpGet("venue-requests")]
        public async Task<IActionResult> GetVenueOwnershipRequests()
        {
            var requests = await _adminService.GetVenueOwnershipRequestsAsync();
            return Ok(requests);
        }

        [HttpPost("venue-requests/{userId}/approve")]
        public async Task<IActionResult> ApproveVenueRequest(int userId)
        {
            await _adminService.ApproveVenueOwnershipRequestAsync(userId);
            return Ok("Venue request approved");
        }

        [HttpPost("venue-requests/{userId}/reject")]
        public async Task<IActionResult> RejectVenueRequest(int userId)
        {
            await _adminService.RejectVenueOwnershipRequestAsync(userId);
            return Ok("Venue request rejected");
        }

    }
}
