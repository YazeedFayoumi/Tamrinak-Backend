using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.BookingDtos
{
    public class AddBookingDto
    {
        [Required]
        public int FieldId { get; set; }

        [Required,DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Range(1, 25)]
        public int NumberOfPeople { get; set; } = 1;
    }
}
