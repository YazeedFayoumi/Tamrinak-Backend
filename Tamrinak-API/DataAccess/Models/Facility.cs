using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
    public class Facility
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FacilityId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public decimal? PricePerHour { get; set; }

        public bool? IsAvailable { get; set; } = true;

        public string? Description { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public int TotalReviews { get; set; }

        public FacilityType Type { get; set; }
        public ICollection<Field> Fields { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();


    }
    public enum FacilityType
    {
        FootballClub,
        Gym,
        BasketballClub,
        TennisClub,
        SwimmingClub
    }

}
