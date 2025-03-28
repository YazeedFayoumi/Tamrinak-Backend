using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
    public class Field
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FieldId { get; set; }
        public int SportId { get; set; }
        public decimal? PricePerHour { get; set; }

        [Required]
        public int? Capacity { get; set; }

        public bool HasLighting { get; set; }
        

        public bool IsIndoor { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public int TotalReviews { get; set; }

        [Required]
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
        
        public ICollection<SportField> SportFields { get; set; }
        public List<Booking> Bookings { get; set; } = new();
    }
}
