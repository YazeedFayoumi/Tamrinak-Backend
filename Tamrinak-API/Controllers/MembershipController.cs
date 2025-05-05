using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO.MembershipDtos;
using Tamrinak_API.Services.MembershipService;

namespace Tamrinak_API.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]

    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        
        [HttpPost("Add")]
        public async Task<IActionResult> AddMembership([FromBody] AddMembershipDto dto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _membershipService.AddMembershipAsync(dto, email);
            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserMemberships()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var memberships = await _membershipService.GetUserMembershipsAsync(email);
            return Ok(memberships);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(id);
            return Ok(membership);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelMembership(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await _membershipService.CancelMembershipAsync(id, email);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await _membershipService.DeleteMembershipAsync(id, email);
            return Ok("Membership deleted");
        }
    }

}
