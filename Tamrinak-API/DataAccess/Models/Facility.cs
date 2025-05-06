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
        public string LocationDesc { get; set; }
        public string? LocationMap { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public decimal? PricePerMonth { get; set; }

        public int? OfferDurationInMonths { get; set; } 
        public decimal? OfferPrice { get; set; } 
        public bool? IsAvailable { get; set; } = true;

        [Required]
        public TimeOnly OpenTime { get; set; }
        [Required]
        public TimeOnly CloseTime { get; set; }
        
        public string? Description { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public int TotalReviews { get; set; }

        public FacilityType Type { get; set; }
        public ICollection<Field> Fields { get; set; }
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<SportFacility> SportFacilities { get; set;} = new List<SportFacility>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<MembershipOffer> MembershipOffers { get; set; } = new List<MembershipOffer>();
    }

    public enum FacilityType
    {
        FootballClub,
        Gym,
        BasketballClub,
        TennisClub,
        SwimmingClub,
        MartialArts,
        Other
    }

}
