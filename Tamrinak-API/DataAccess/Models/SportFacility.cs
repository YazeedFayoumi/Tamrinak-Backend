using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DataAccess.Models
{
    public class SportFacility
    {
        [Required]
        public int FacilityId { get; set; }

        
        public Facility Facility { get; set; }

        [Required]
        public int SportId { get; set; }

        
        public Sport Sport { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
