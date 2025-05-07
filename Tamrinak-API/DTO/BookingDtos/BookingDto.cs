using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.BookingDtos
{
    public class BookingDto
    {
        public int BookingId { get; set; }//TODO
        public int? FieldId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; }
        public int NumberOfPeople { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set;}
    }
}
