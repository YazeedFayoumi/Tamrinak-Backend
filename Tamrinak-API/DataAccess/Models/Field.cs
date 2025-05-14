using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Field
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FieldId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int SportId { get; set; }


		[Required]
		public required string LocationDesc { get; set; }
		public string? LocationMap { get; set; }

		[Phone]
		public string? PhoneNumber { get; set; }

		[Required]
		public TimeOnly OpenTime { get; set; }
		[Required]
		public TimeOnly CloseTime { get; set; }

		public bool IsAvailable { get; set; }

		public int? Capacity { get; set; }

		public decimal? PricePerHour { get; set; }

		public bool? HasLighting { get; set; }


		public bool? IsIndoor { get; set; }

		[NotMapped]
		public double AverageRating { get; set; }

		[NotMapped]
		public int TotalReviews { get; set; }

        public int? OwnerId { get; set; }
        public User? Owner { get; set; }

        public int? FacilityId { get; set; }
		public Facility Facility { get; set; }

		public Sport Sport { get; set; }
		public List<Booking> Bookings { get; set; } = new();
		public ICollection<Image> Images { get; set; } = new List<Image>();
	}
}
