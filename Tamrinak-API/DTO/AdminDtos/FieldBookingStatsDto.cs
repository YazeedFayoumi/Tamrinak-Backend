namespace Tamrinak_API.DTO.AdminDtos
{
    public class FieldBookingStatsDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public int TotalBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int PastBookings { get; set; }
        public int CanceledBookings { get; set; }
        public int UniqueUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<BookingByDateDto> UpcomingSchedule { get; set; }
        public List<AdminBookingDto> Bookings { get; set; } // 👈 raw data
    }
}
