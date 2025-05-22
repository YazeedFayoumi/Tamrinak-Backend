using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DTO.BookingDtos;
using Tamrinak_API.Services.BookingService;

namespace Tamrinak_API.Controllers
{
	//[Authorize(Roles = "User")]//TODO
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly IBookingService _bookingService;
		public BookingController(IBookingService bookingService)
		{
			_bookingService = bookingService;
		}

		[HttpPost("book-field"), Authorize]
		public async Task<IActionResult> CreateFieldBooking(AddBookingDto dto)
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;
				var booking = await _bookingService.CreateFieldBookingAsync(dto, email);
				return Ok(booking);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("booking/{bookingId}")]
		public async Task<IActionResult> GetBooking(int bookingId)
		{
			try
			{
				var booking = await _bookingService.GetBookingByIdAsync(bookingId);
				if (booking == null)
					return NotFound();

				return Ok(booking);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("user-bookings/{userId}")]
		public async Task<IActionResult> GetUserBookings(int userId)
		{
			try
			{
				var bookings = await _bookingService.GetUserBookingsAsync(userId);
				return Ok(bookings);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("booking/{bookingId}")]
		public async Task<IActionResult> DeleteBooking(int bookingId)
		{
			try
			{
				var success = await _bookingService.DeleteBookingAsync(bookingId);
				if (!success)
					return NotFound();

				return Ok("Booking deleted successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("cancel-booking/{bookingId}")]
		public async Task<IActionResult> CancelBooking(int bookingId)
		{
			try
			{
				var success = await _bookingService.CancelBookingAsync(bookingId);
				if (!success)
					return NotFound();

				return Ok("Booking Cancelled successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch("{bookingId}/pay")]
		public async Task<IActionResult> MarkBookingAsPaid(int bookingId)
		{
			try
			{
				var success = await _bookingService.MarkBookingAsPaidAsync(bookingId);
				if (!success)
					return NotFound();

				return Ok("Booking marked as paid.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("booking/{id}")]
		public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
		{
			try
			{
				var result = await _bookingService.ChangeBookingAsync(id, dto);
				return result == null ? Ok("Booking updated successfully.") : BadRequest(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/*     [HttpGet("fields/{fieldId}/available-slots")]
             public async Task<IActionResult> GetAvailableSlots(int fieldId, [FromQuery] DateTime bookingDate)
             {
                 var slots = await _bookingService.GetAvailableTimeSlotsWithNextDayAsync(fieldId, bookingDate);
                 var result = slots.Select(s => new
                 {
                     Start = s.Start.ToString("HH:mm"),
                     End = s.End.ToString("HH:mm")
                 });

                 return Ok(result);
             }*/

		[HttpGet("by-date")]
		public async Task<IActionResult> GetBookingsByDate(int fieldId, DateTime date)
		{
			try
			{
				var result = await _bookingService.GetBookingsByDateAsync(fieldId, date);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("availability")] // shows from 00:00 to 03:00 the next day 
		public async Task<IActionResult> GetFieldAvailability(int fieldId, DateTime date)
		{
			try
			{
				var result = await _bookingService.GetAvailableTimeSlotsWithNextDayAsync(fieldId, date);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}

