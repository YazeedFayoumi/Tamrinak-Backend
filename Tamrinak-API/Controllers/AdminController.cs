using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.Services.AdminService;
using Tamrinak_API.Services.EmailService;
using Tamrinak_API.Services.UserService;

namespace Tamrinak_API.Controllers
{
	/// <summary>
	/// /////////
	/// </summary>
	//[Authorize(Roles = "SuperAdmin")]
	[ApiController]
	[Route("api/admin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _adminService;
		private readonly IEmailService _emailService;
		public AdminController(IAdminService adminService, IEmailService emailService)
		{
			_adminService = adminService;
			_emailService = emailService;
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var users = await _adminService.GetAllUsersAsync();
				return Ok(users);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("users/{id}")]
		public async Task<IActionResult> GetUserById(int id)
		{
			try
			{
				var user = await _adminService.GetUserByIdAsync(id);
				return user != null ? Ok(user) : NotFound("User not found");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("users/{id}/add-role")]
		public async Task<IActionResult> AddRoleToUser(int id, [FromBody] string roleName)
		{
			try
			{
				await _adminService.AddRoleToUserAsync(id, roleName);
				return Ok("Role added");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("users/{id}/remove-role")]
		public async Task<IActionResult> RemoveRoleFromUser(int id, [FromBody] string roleName)
		{
			try
			{
				await _adminService.RemoveRoleToUserAsync(id, roleName);
				return Ok("Role removed");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("venue-requests")]
		public async Task<IActionResult> GetVenueOwnershipRequests()
		{
			try
			{
				var requests = await _adminService.GetVenueOwnershipRequestsAsync();
				return Ok(requests);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("venue-requests/{userId}/approve")]
		public async Task<IActionResult> ApproveVenueRequest(int userId)
		{
			try
			{
				await _adminService.ApproveVenueOwnershipRequestAsync(userId);
				return Ok("Venue request approved");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("venue-requests/{userId}/reject")]
		public async Task<IActionResult> RejectVenueRequest(int userId)
		{
			try
			{
				await _adminService.RejectVenueOwnershipRequestAsync(userId);
				return Ok("Venue request rejected");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("users/{id}/bookings")]
		public async Task<IActionResult> GetUserBookings(int id)
		{
			try
			{
				var bookings = await _adminService.GetUserBookingsAsync(id);
				return Ok(bookings);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("users/{id}/memberships")]
		public async Task<IActionResult> GetUserMemberships(int id)
		{
			try
			{
				var memberships = await _adminService.GetUserMembershipsAsync(id);
				return Ok(memberships);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("fields/{fieldId}/bookings/stats")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetFieldBookingStats(int fieldId)
		{
			try
			{
				var stats = await _adminService.GetFieldBookingStatsAsync(fieldId);
				return Ok(stats);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("facilities/{facilityId}/memberships/stats")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetFacilityMembershipStats(int facilityId)
		{
			try
			{
				var stats = await _adminService.GetFacilityMembershipStatsAsync(facilityId);
				return Ok(stats);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("venue-managers")]
		[Authorize(Roles = "Admin, SuperAdmin")]
		public async Task<IActionResult> GetVenueManagers()
		{
			try
			{
				var data = await _adminService.GetVenueManagerOverviewsAsync();
				return Ok(data);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("reviews")]
		public async Task<IActionResult> GetAllReviews(
		[FromQuery] bool? isVerified,
		[FromQuery] int? facilityId,
		[FromQuery] int? fieldId,
		[FromQuery] int page,
		[FromQuery] int pageSize)
		{
			try
			{
				var result = await _adminService.GetAllReviewsAsync(isVerified, facilityId, fieldId, page, pageSize);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("reviews/{id}")]
		public async Task<IActionResult> GetReviewById(int id)
		{
			try
			{
				var review = await _adminService.GetReviewByIdAsync(id);
				return review != null ? Ok(review) : NotFound("Review not found");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("reviews/{id}/verify")]
		public async Task<IActionResult> VerifyReview(int id)
		{
			try
			{
				await _adminService.VerifyReviewAsync(id);
				return Ok("Review verified.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("reviews/{id}")]
		public async Task<IActionResult> DeleteReview(int id)
		{
			try
			{
				await _adminService.DeleteReviewAsync(id);
				return Ok("Review deleted.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("reviews/{id}/replies")]
		public async Task<IActionResult> GetRepliesForReview(int id)
		{
			try
			{
				var replies = await _adminService.GetReviewRepliesAsync(id);
				return Ok(replies);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("reviews/{id}/reset-likes")]
		public async Task<IActionResult> ResetReviewLikes(int id)
		{
			try
			{
				await _adminService.ResetReviewLikesAsync(id);
				return Ok("Likes reset.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("reviews/{id}/lock")]
		public async Task<IActionResult> LockReviewThread(int id)
		{
			try
			{
				await _adminService.SetReviewLockStatusAsync(id, true);
				return Ok("Review thread locked.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("reviews/{id}/unlock")]
		public async Task<IActionResult> UnlockReviewThread(int id)
		{
			try
			{
				await _adminService.SetReviewLockStatusAsync(id, false);
				return Ok("Review thread unlocked.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("reviews/bulk-verify")]
		public async Task<IActionResult> BulkVerifyReviews([FromBody] List<int> reviewIds)
		{
			try
			{
				await _adminService.BulkVerifyReviewsAsync(reviewIds);
				return Ok("Reviews verified successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("users/{userId}/reviews")]
		public async Task<IActionResult> GetUserReviews(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			try
			{
				var result = await _adminService.GetReviewsByUserAsync(userId, page, pageSize);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("payments")]
		public async Task<IActionResult> GetPayments([FromQuery] string? method, [FromQuery] bool? confirmed)
		{
			try
			{
				var payments = await _adminService.GetAllPaymentsAsync(method, confirmed);
				return Ok(payments);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("payments/{id}")]
		public async Task<IActionResult> GetPaymentById(int id)
		{
			try
			{
				var payment = await _adminService.GetPaymentByIdAsync(id);
				return payment != null ? Ok(payment) : NotFound("Payment not found");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("payments/{id}/confirm")]
		public async Task<IActionResult> ConfirmPayment(int id)
		{
			try
			{
				await _adminService.ConfirmPaymentAsync(id);
				return Ok("Payment confirmed.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("payments/{id}/refund")]
		public async Task<IActionResult> RefundPayment(int id)
		{
			try
			{
				await _adminService.RefundPaymentAsync(id);
				return Ok("Payment refunded.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("users/{id}/payments")]
		public async Task<IActionResult> GetUserPayments(int id)
		{
			try
			{
				var payments = await _adminService.GetPaymentsByUserAsync(id);
				return Ok(payments);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

        ////
        [HttpPost("contact")]
        [AllowAnonymous]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest("All fields are required.");
            }

            await _emailService.SendContactMessageAsync(dto);
            return Ok("Your message has been sent.");
        }


    }
}
