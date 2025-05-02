using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("book-field")]
        public async Task<IActionResult> CreateFieldBooking(AddBookingDto dto)
        {
            var booking = await _bookingService.CreateFieldBookingAsync(dto);
            return Ok(booking);
        }

        [HttpGet("get-booking/{bookingId}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpGet("user-bookings/{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpDelete("delete-booking/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var success = await _bookingService.DeleteBookingAsync(bookingId);
            if (!success)
                return NotFound();

            return Ok("Booking deleted successfully.");
        }

        [HttpPatch("cancel-booking/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var success = await _bookingService.CancelBookingAsync(bookingId);
            if (!success)
                return NotFound();

            return Ok("Booking Cancelled successfully.");
        }

        [HttpPatch("{bookingId}/pay")]
        public async Task<IActionResult> MarkBookingAsPaid(int bookingId)
        {
            var success = await _bookingService.MarkBookingAsPaidAsync(bookingId);
            if (!success)
                return NotFound();

            return Ok("Booking marked as paid.");
        }

        [HttpPut("change-booking/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
        {
            var result = await _bookingService.ChangeBookingAsync(id, dto);
            return result == null ? Ok("Booking updated successfully.") : BadRequest(result);
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetFieldAvailability(int fieldId, AvailabilityDto dto)//!!??
        {
            var slots = await _bookingService.GetAvailableTimeSlotsAsync(fieldId, dto);
            return Ok(slots);
        }
    }
}

