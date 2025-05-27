using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.BookingDtos;
using Tamrinak_API.Repository.GenericRepo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tamrinak_API.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly IGenericRepo<Booking> _bookingRepo;
		private readonly IGenericRepo<Field> _fieldRepo;
		private readonly IGenericRepo<User> _userRepo;
		public BookingService(IGenericRepo<Booking> bookingRepo, IGenericRepo<Field> fieldRepo, IGenericRepo<User> userRepo)
		{
			_bookingRepo = bookingRepo;
			_fieldRepo = fieldRepo;
			_userRepo = userRepo;
		}

		public async Task<BookingDto> CreateFieldBookingAsync(AddBookingDto dto, string email)
		{

			if (string.IsNullOrEmpty(email))
				throw new UnauthorizedAccessException();

			var user = await _userRepo.GetByConditionAsync(u => u.Email == email);

			if (!user.IsEmailConfirmed)
				throw new InvalidOperationException("Please verify your email to continue.");

			// Now use user.UserId to create the booking

			var validationError = await ValidateBookingAsync(dto);
			if (validationError != null)
				throw new Exception(validationError);

			var field = await _fieldRepo.GetAsync(dto.FieldId);


			/*decimal noP = dto.NumberOfPeople;
			if (field.PricePerHour == null)
				throw new Exception("Start time and end time are required.");*/

			var start = TimeOnly.Parse(dto.StartTime);
			var end = TimeOnly.Parse(dto.EndTime);

			decimal price = (decimal)field.PricePerHour.Value;
			decimal duration = (decimal)(end - start).TotalHours;
		    decimal totalCost =  price * duration;//noP *

			var booking = new Booking
			{
				UserId = user.UserId,
				FieldId = dto.FieldId,
				BookingDate = dto.BookingDate,
				StartTime = start,
				EndTime = end,
				TotalCost = totalCost,
				NumberOfPeople = dto.NumberOfPeople,
				Status = BookingStatus.Pending,
				IsPaid = false,
				CreatedAt = DateTime.UtcNow

			};

			var newBooking = await _bookingRepo.CreateAsync(booking);
			return new BookingDto
			{
				BookingId = newBooking.BookingId,
				BookingDate = newBooking.BookingDate,
				UserId = newBooking.UserId,
				FieldId = newBooking.FieldId,
				Status = newBooking.Status,
				StartTime = newBooking.StartTime.ToString("HH:mm"),
				EndTime = newBooking.EndTime.ToString("HH:mm"),
				TotalCost = totalCost,
				NumberOfPeople = newBooking.NumberOfPeople,
				Duration = newBooking.Duration,
				CreatedAt = newBooking.CreatedAt
			};
		}

		public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
		{
			var booking = await _bookingRepo.GetByConditionIncludeAsync(
				b => b.BookingId == bookingId,
				q => q.Include(b => b.Field).Include(b => b.User)
			);

			if (booking == null) return null;

			return new BookingDto
			{
				BookingId = booking.BookingId,
				FieldId = booking.FieldId,
				UserId = booking.UserId,
				BookingDate = booking.BookingDate,
				StartTime = booking.StartTime.ToString("HH:mm"),
				EndTime = booking.EndTime.ToString("HH:mm"),
				TotalCost = booking.TotalCost,
				IsPaid = booking.IsPaid,
				NumberOfPeople = booking.NumberOfPeople,
				Duration = booking.Duration,
				Status = booking.Status,
				CreatedAt = booking.CreatedAt,
				CancelledAt = booking.CancelledAt
			};
		}

		public async Task<List<BookingDto>> GetUserBookingsAsync(int userId)
		{
			var bookings = await _bookingRepo.GetListByConditionIncludeAsync(
				b => b.UserId == userId,
				q => q.Include(b => b.Field)
			);

			return bookings.Select(b => new BookingDto
			{
				BookingId = b.BookingId,
				FieldId = b.FieldId,
				UserId = b.UserId,
				BookingDate = b.BookingDate,
				StartTime = b.StartTime.ToString("HH:mm"),
				EndTime = b.EndTime.ToString("HH:mm"),
				TotalCost = b.TotalCost,
				IsPaid = b.IsPaid,
				NumberOfPeople = b.NumberOfPeople,
				Duration = b.Duration,
				Status = b.Status,
				CreatedAt = b.CreatedAt,
				CancelledAt = b.CancelledAt

			}).ToList();
		}

		public async Task<bool> DeleteBookingAsync(int bookingId)
		{
			var booking = await _bookingRepo.GetAsync(bookingId);
			if (booking == null) return false;

			await _bookingRepo.DeleteAsync(booking);
			return true;
		}

		public async Task<bool> CancelBookingAsync(int bookingId)
		{
			var booking = await _bookingRepo.GetAsync(bookingId);
			if (booking == null) return false;

			booking.Status = BookingStatus.Cancelled;
			booking.CancelledAt = DateTime.UtcNow;
			await _bookingRepo.UpdateAsync(booking);
			return true;
		}

		public async Task<string?> ChangeBookingAsync(int bookingId, UpdateBookingDto dto)
		{
			var booking = await _bookingRepo.GetAsync(bookingId);
			var start = TimeOnly.Parse(dto.StartTime);
			var end = TimeOnly.Parse(dto.EndTime);
			if (booking == null)
				return "Booking not found.";

			if (booking.Status == BookingStatus.Cancelled)
				return "Cannot edit a cancelled booking.";

			if (booking.BookingDate < DateTime.Now.Date ||
				(booking.BookingDate == DateTime.Now.Date && start <= TimeOnly.FromDateTime(DateTime.Now)))
				return "Cannot modify a past or ongoing booking.";

			int id = (int)booking.FieldId;
			var field = await _fieldRepo.GetAsync(id);
			if (field == null) return "Field not found.";

			//TODO: check feild capasety before updating no. of people

			var validationError = await ValidateBookingAsync(new AddBookingDto
			{
				FieldId = field.FieldId,
				BookingDate = dto.BookingDate,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				NumberOfPeople = dto.NumberOfPeople
			});

			if (validationError != null)
				return validationError;

			decimal duration = (decimal)(end > start
				? end - start
				: end.AddHours(24) - start).TotalHours;

			decimal totalCost = field.PricePerHour.Value * dto.NumberOfPeople * (decimal)duration;

			// Update the booking
			booking.BookingDate = dto.BookingDate;
			booking.StartTime = start;
			booking.EndTime = end;
			booking.NumberOfPeople = dto.NumberOfPeople;
			booking.TotalCost = totalCost;
			booking.Status = BookingStatus.Pending;
			booking.IsPaid = false;

			await _bookingRepo.UpdateAsync(booking);
			return null;
		}

		public async Task<bool> MarkBookingAsPaidAsync(int bookingId)
		{
			var booking = await _bookingRepo.GetAsync(bookingId);
			if (booking == null) return false;

			booking.IsPaid = true;
			booking.Status = BookingStatus.Completed;
			await _bookingRepo.UpdateAsync(booking);
			return true;
		}

        public async Task<List<BookingDto>> GetBookingsByDateAsync(int fieldId, DateTime date)
        {
            var bookings = await _bookingRepo.GetListByConditionIncludeAsync(
                b =>
                    b.FieldId == fieldId &&
                    (
                        b.BookingDate == date ||                             // same day
                        (b.StartTime > b.EndTime && b.BookingDate == date.AddDays(-1)) // overnight from previous day
                    ),
                q => q.Include(b => b.User) // or include Field if needed
            );

            return bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                FieldId = b.FieldId,
                UserId = b.UserId,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime.ToString("HH:mm"),
                EndTime = b.EndTime.ToString("HH:mm"),
                TotalCost = b.TotalCost,
                IsPaid = b.IsPaid,
                NumberOfPeople = b.NumberOfPeople,
                Duration = b.Duration,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                CancelledAt = b.CancelledAt
            }).ToList();
        }

        public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsWithNextDayAsync(int fieldId, DateTime date)
        {
            var field = await _fieldRepo.GetAsync(fieldId)
				   ?? throw new Exception("Field not found");

            if (date.Date < DateTime.UtcNow.Date)
                throw new Exception("Cannot check availability for past dates.");

            if (date.Date > DateTime.UtcNow.Date.AddDays(90))
                throw new Exception("You can only view availability up to 90 days in advance.");

            var open = field.OpenTime;
            var close = field.CloseTime;
            bool isOpen24Hours = open == TimeOnly.MinValue && close == TimeOnly.MaxValue;

            var availabilityStart = date.Date.Add(open.ToTimeSpan());
            var availabilityEnd = open < close
                ? date.Date.Add(close.ToTimeSpan())
                : date.Date.AddDays(1).Add(close.ToTimeSpan());

            var extendedEnd = availabilityEnd.AddHours(3);
            var slotDuration = TimeSpan.FromMinutes(30);

            // Load relevant bookings
            var bookings = await _bookingRepo.GetListByConditionAsync(
                b => b.FieldId == fieldId &&
                    (b.BookingDate == date || b.BookingDate == date.AddDays(1) || b.BookingDate == date.AddDays(-1))&&
				   b.Status != BookingStatus.Cancelled
            );

            // Build list of blocked time ranges
            var blockedRanges = bookings.Select(b =>
            {
                var bStart = b.BookingDate.Add(b.StartTime.ToTimeSpan());
                var bEnd = b.StartTime < b.EndTime
                    ? b.BookingDate.Add(b.EndTime.ToTimeSpan())
                    : b.BookingDate.AddDays(1).Add(b.EndTime.ToTimeSpan());

                return (Start: bStart, End: bEnd);
            }).ToList();

            // Scan through all time and merge consecutive free ranges
            var availableRanges = new List<TimeSlotDto>();

            DateTime? currentStart = null;

            for (var time = availabilityStart; time < extendedEnd; time += slotDuration)
            {
                var timeEnd = time.Add(slotDuration);

                // Skip if outside operating hours (if not open 24)
                if (!isOpen24Hours && !IsWithinOperatingHours(TimeOnly.FromDateTime(time), TimeOnly.FromDateTime(timeEnd), open, close))
                    continue;

                // Is this slot overlapping any existing booking?
                bool isBlocked = blockedRanges.Any(r => time < r.End && timeEnd > r.Start);

                if (isBlocked)
                {
                    // If we were building an open range, close it
                    if (currentStart.HasValue)
                    {
                        availableRanges.Add(new TimeSlotDto
                        {
                            StartTime = currentStart.Value.ToString("HH:mm"),
                            EndTime = time.ToString("HH:mm")
                        });
                        currentStart = null;
                    }
                }
                else
                {
                    // Start a new open range if not already in one
                    if (!currentStart.HasValue)
                        currentStart = time;
                }
            }

            // If still open range at the end, close it
            if (currentStart.HasValue)
            {
                availableRanges.Add(new TimeSlotDto
                {
                    StartTime = currentStart.Value.ToString("HH:mm"),
                    EndTime = extendedEnd.ToString("HH:mm")
                });
            }

            return availableRanges;
        }

    


    private async Task<string?> ValidateBookingAsync(AddBookingDto bookingDto)
		{
			var start = TimeOnly.Parse(bookingDto.StartTime);
			var end = TimeOnly.Parse(bookingDto.EndTime);

			var bookingStartDateTime = bookingDto.BookingDate.Add(start.ToTimeSpan());
			if (bookingStartDateTime < DateTime.Now)
				return "Booking date cannot be in the past.";

			int maxBookingDaysAhead = 90; // Or make this configurable via appsettings
			if (bookingDto.BookingDate.Date > DateTime.UtcNow.Date.AddDays(maxBookingDaysAhead))
			{
				return $"You can only book up to {maxBookingDaysAhead} days in advance.";
			}

			var field = await _fieldRepo.GetAsync(bookingDto.FieldId);
			if (field == null)
				return "Field does not exist.";
			if (!field.IsAvailable)
			{
				return "Field not available.";

            }
			if (field.Capacity < bookingDto.NumberOfPeople)
				return "Number of players exceeds field capacity. عدد اللاعبين يتعدى استيعاب المرفق";

			bool isOpen24Hours = field.OpenTime == TimeOnly.MinValue && field.CloseTime == TimeOnly.MaxValue;

			// Validate time input
			if (bookingDto.StartTime == bookingDto.EndTime)
				return "Start and end time cannot be the same.";

			if (!isOpen24Hours)
			{
				if (!IsWithinOperatingHours(start, end, field.OpenTime, field.CloseTime))
				{
					return $"Booking time must be within the field's working hours ({field.OpenTime:hh\\:mm} - {field.CloseTime:hh\\:mm}).";
				}
			}

			// Calculate duration
			TimeSpan duration = start < end
				? end - start
				: (TimeOnly.MaxValue - start + end.ToTimeSpan() + TimeSpan.FromSeconds(1)); // accounts for overnight

			if (duration.TotalMinutes < 30 || duration.TotalHours > 4)
				return "Booking duration must be between 30 minutes and 4 hours.";

			// Build absolute DateTime ranges
			// Step 1: Convert input times to DateTime
			var startDateTime = bookingDto.BookingDate.Add(start.ToTimeSpan());
			var endDateTime = start < end
				? bookingDto.BookingDate.Add(end.ToTimeSpan())
				: bookingDto.BookingDate.AddDays(1).Add(end.ToTimeSpan());

			// Step 2: Get all bookings for the same field and date (and next day in case of overnight)
			var relevantBookings = await _bookingRepo.GetListByConditionAsync(
				b => b.FieldId == bookingDto.FieldId &&
					 (b.BookingDate == bookingDto.BookingDate || b.BookingDate == bookingDto.BookingDate.AddDays(1) ||
					 b.BookingDate == bookingDto.BookingDate.AddDays(-1))&&
				     b.Status != BookingStatus.Cancelled
            );

			bool isConflict = relevantBookings.Any(b =>
			{
				var bStart = b.BookingDate.Add(b.StartTime.ToTimeSpan());
				var bEnd = b.StartTime < b.EndTime
					? b.BookingDate.Add(b.EndTime.ToTimeSpan())
					: b.BookingDate.AddDays(1).Add(b.EndTime.ToTimeSpan());

				return
					(startDateTime >= bStart && startDateTime < bEnd) ||
					(endDateTime > bStart && endDateTime <= bEnd) ||
					(startDateTime <= bStart && endDateTime >= bEnd);
			});

			return isConflict ? "The selected time slot is already booked." : null;
		}

		private bool IsWithinOperatingHours(TimeOnly start, TimeOnly end, TimeOnly open, TimeOnly close)
		{
			if (start < end)
				return start >= open && end <= close;

			return start >= open || end <= close;
		}

	}
}
