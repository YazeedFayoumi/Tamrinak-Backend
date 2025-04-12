using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
    public class Field
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FieldId { get; set; }
        public int SportId { get; set; }
        
        [Required]
        public string Location { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
     
        public decimal? PricePerHour { get; set; }

        public bool? HasLighting { get; set; }
        

        public bool IsIndoor { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public int TotalReviews { get; set; }

        
        public int? FacilityId { get; set; }
        public Facility Facility { get; set; }
        
        public Sport Sport { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
