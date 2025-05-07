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
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }

        [Range(1, 25)]
        public int NumberOfPeople { get; set; } = 1;
    }
}
