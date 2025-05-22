using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO.ReviewDtos;
using Tamrinak_API.Services.ReviewService;

namespace Tamrinak_API.Controllers
{
	[ApiController]
	[Route("api/review")]
	public class ReviewController : ControllerBase
	{
		private readonly IReviewService _reviewService;

		public ReviewController(IReviewService reviewService)
		{
			_reviewService = reviewService;
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				var review = await _reviewService.AddReviewAsync(dto, email);
				return Ok(review);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("field/{fieldId}")]
		public async Task<IActionResult> GetFieldReviews(int fieldId)
		{
			try
			{
				var reviews = await _reviewService.GetReviewsForFieldAsync(fieldId);
				return Ok(reviews);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("facility/{facilityId}")]
		public async Task<IActionResult> GetFacilityReviews(int facilityId)
		{
			try
			{
				var reviews = await _reviewService.GetReviewsForFacilityAsync(facilityId);
				return Ok(reviews);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{reviewId}")]
		public async Task<IActionResult> GetReviewById(int reviewId)
		{
			try
			{
				var review = await _reviewService.GetReviewByIdAsync(reviewId);
				return Ok(review);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("my-reviews")]
		[Authorize]
		public async Task<IActionResult> GetMyReviews()
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(email))
					return Unauthorized();

				var reviews = await _reviewService.GetUserReviewsAsync(email);
				return Ok(reviews);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{reviewId}")]
		[Authorize]
		public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				var updated = await _reviewService.UpdateReviewAsync(reviewId, dto, email);
				return Ok(updated);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{reviewId}")]
		[Authorize]
		public async Task<IActionResult> DeleteReview(int reviewId)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				var result = await _reviewService.DeleteReviewAsync(reviewId, email);
				return result ? Ok("Deleted") : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{reviewId}/like")]
		public async Task<IActionResult> LikeReview(int reviewId)
		{
			try
			{
				var liked = await _reviewService.LikeReviewAsync(reviewId);
				return liked ? Ok("Liked") : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("reply")]
		[Authorize]
		public async Task<IActionResult> AddReply([FromBody] AddReplyDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(email))
					return Unauthorized();

				var reply = await _reviewService.AddReplyAsync(dto, email);
				return Ok(reply);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{reviewId}/replies")]
		public async Task<IActionResult> GetReplies(int reviewId)
		{
			try
			{
				var replies = await _reviewService.GetRepliesAsync(reviewId);
				return Ok(replies);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("reply/{replyId}")]
		[Authorize]
		public async Task<IActionResult> UpdateReply(int replyId, [FromBody] UpdateReplyDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				await _reviewService.UpdateReplyAsync(replyId, email, dto);
				return Ok("Reply updated");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("reply/{replyId}")]
		[Authorize]
		public async Task<IActionResult> DeleteReply(int replyId)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				await _reviewService.DeleteReplyAsync(replyId, email);
				return Ok("Reply deleted");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}

}
