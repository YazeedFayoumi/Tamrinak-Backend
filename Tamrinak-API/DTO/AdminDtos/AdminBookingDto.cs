using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminBookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }          // From User
        public string? UserEmail { get; set; }

        public string? FieldName { get; set; }        // From Field (nullable)
        public string? FacilityName { get; set; }     // From Facility (nullable)
        public DateTime BookingDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
    }
}
