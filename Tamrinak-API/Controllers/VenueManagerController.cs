using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.Services.ImageService;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VenueManagerController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IImageService _imageService;
		public VenueManagerController(IUserService userService, IImageService imageService)
		{
			_userService = userService;
			_imageService = imageService;
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
