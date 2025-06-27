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

		[HttpPost("new")]
		public async Task<IActionResult> AddMembership([FromBody] AddMembershipDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(email))
				{
					return Unauthorized("User email not found in token");
				}

				var result = await _membershipService.AddMembershipAsync(dto, email);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("user")]
		public async Task<IActionResult> GetUserMemberships()
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				var memberships = await _membershipService.GetUserMembershipsAsync(email);
				return Ok(memberships);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetMembershipById(int id)
		{
			try
			{
				var membership = await _membershipService.GetMembershipByIdAsync(id);
				return Ok(membership);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> CancelMembership(int id)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				await _membershipService.CancelMembershipAsync(id, email);
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMembership(int id)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				await _membershipService.DeleteMembershipAsync(id, email);
				return Ok("Membership deleted");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}

}
