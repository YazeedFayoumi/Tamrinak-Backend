using Tamrinak_API.DTO.BookingDtos;

namespace Tamrinak_API.Services.BookingService
{
	public interface IBookingService
	{
		Task<BookingDto> CreateFieldBookingAsync(AddBookingDto dto, string email);
		Task<BookingDto?> GetBookingByIdAsync(int bookingId);
		Task<List<BookingDto>> GetUserBookingsAsync(int userId);
		Task<bool> DeleteBookingAsync(int bookingId);
		Task<bool> CancelBookingAsync(int bookingId);
		Task<string?> ChangeBookingAsync(int bookingId, UpdateBookingDto dto);
		Task<bool> MarkBookingAsPaidAsync(int bookingId);
		Task<List<(TimeOnly Start, TimeOnly End)>> GetAvailableTimeSlotsAsync(int fieldId, AvailabilityDto dto);

	}
}
