using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.Services.AdminService;
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

        [HttpGet("users/{id}/bookings")]
        public async Task<IActionResult> GetUserBookings(int id)
        {
            var bookings = await _adminService.GetUserBookingsAsync(id);
            return Ok(bookings);
        }

        [HttpGet("users/{id}/memberships")]
        public async Task<IActionResult> GetUserMemberships(int id)
        {
            var memberships = await _adminService.GetUserMembershipsAsync(id);
            return Ok(memberships);
        }

        [HttpGet("fields/{fieldId}/bookings/stats")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFieldBookingStats(int fieldId)
        {
            var stats = await _adminService.GetFieldBookingStatsAsync(fieldId);
            return Ok(stats);
        }

        [HttpGet("facilities/{facilityId}/memberships/stats")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFacilityMembershipStats(int facilityId)
        {
            var stats = await _adminService.GetFacilityMembershipStatsAsync(facilityId);
            return Ok(stats);
        }

        [HttpGet("venue-managers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVenueManagers()
        {
            var data = await _adminService.GetVenueManagerOverviewsAsync();
            return Ok(data); 
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReviews(
        [FromQuery] bool? isVerified,
        [FromQuery] int? facilityId,
        [FromQuery] int? fieldId,
        [FromQuery] int page,
        [FromQuery] int pageSize)
            {
            var result = await _adminService.GetAllReviewsAsync(isVerified, facilityId, fieldId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("reviews/{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _adminService.GetReviewByIdAsync(id);
            return review != null ? Ok(review) : NotFound("Review not found");
        }

        [HttpPatch("reviews/{id}/verify")]
        public async Task<IActionResult> VerifyReview(int id)
        {
            await _adminService.VerifyReviewAsync(id);
            return Ok("Review verified.");
        }

        [HttpDelete("reviews/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _adminService.DeleteReviewAsync(id);
            return Ok("Review deleted.");
        }

        [HttpGet("reviews/{id}/replies")]
        public async Task<IActionResult> GetRepliesForReview(int id)
        {
            var replies = await _adminService.GetReviewRepliesAsync(id);
            return Ok(replies);
        }

        [HttpPatch("reviews/{id}/reset-likes")]
        public async Task<IActionResult> ResetReviewLikes(int id)
        {
            await _adminService.ResetReviewLikesAsync(id);
            return Ok("Likes reset.");
        }

        [HttpPatch("reviews/{id}/lock")]
        public async Task<IActionResult> LockReviewThread(int id)
        {
            await _adminService.SetReviewLockStatusAsync(id, true);
            return Ok("Review thread locked.");
        }

        [HttpPatch("reviews/{id}/unlock")]
        public async Task<IActionResult> UnlockReviewThread(int id)
        {
            await _adminService.SetReviewLockStatusAsync(id, false);
            return Ok("Review thread unlocked.");
        }

        [HttpPatch("reviews/bulk-verify")]
        public async Task<IActionResult> BulkVerifyReviews([FromBody] List<int> reviewIds)
        {
            await _adminService.BulkVerifyReviewsAsync(reviewIds);
            return Ok("Reviews verified successfully.");
        }

        [HttpGet("users/{userId}/reviews")]
        public async Task<IActionResult> GetUserReviews(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _adminService.GetReviewsByUserAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments([FromQuery] string? method, [FromQuery] bool? confirmed)
        {
            var payments = await _adminService.GetAllPaymentsAsync(method, confirmed);
            return Ok(payments);
        }

        [HttpGet("payments/{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _adminService.GetPaymentByIdAsync(id);
            return payment != null ? Ok(payment) : NotFound("Payment not found");
        }

        [HttpPatch("payments/{id}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            await _adminService.ConfirmPaymentAsync(id);
            return Ok("Payment confirmed.");
        }

        [HttpPatch("payments/{id}/refund")]
        public async Task<IActionResult> RefundPayment(int id)
        {
            await _adminService.RefundPaymentAsync(id);
            return Ok("Payment refunded.");
        }

        [HttpGet("users/{id}/payments")]
        public async Task<IActionResult> GetUserPayments(int id)
        {
            var payments = await _adminService.GetPaymentsByUserAsync(id);
            return Ok(payments);
        }

    }
}
