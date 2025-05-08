using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Review
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ReviewId { get; set; }

		[Required]
		public int UserId { get; set; }
		public User User { get; set; }


		public int? FacilityId { get; set; }
		public Facility Facility { get; set; }

		public int? FieldId { get; set; }
		public Field Field { get; set; }

		[Required]
		[Range(1, 5)]
		public int Rating { get; set; }

		[MaxLength(1000)]
		public string Comment { get; set; }

		public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

		public bool IsVerified { get; set; } = false;
		public int Likes { get; set; } = 0;
	}
}
