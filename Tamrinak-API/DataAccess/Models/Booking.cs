using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FieldId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public decimal TotalCost { get; set; }
        public bool IsPaid { get; set; } = false;

        public User User { get; set; }
        public Field Field { get; set; }
        public Payment Payment { get; set; }
    }
}
