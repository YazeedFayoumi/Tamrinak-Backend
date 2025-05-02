using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.BookingDtos
{
    public class UpdateBookingDto
    {
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        [Range(1, 25)]
        public int NumberOfPeople { get; set; }

    }
}
