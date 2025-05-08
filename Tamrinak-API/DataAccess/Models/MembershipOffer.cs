using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DataAccess.Models
{
	public class MembershipOffer
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OfferId { get; set; }

		[Required]
		public int FacilityId { get; set; }

		[Required]
		public int DurationInMonths { get; set; }

		[Required]
		[Column(TypeName = "decimal(10,2)")]
		public decimal Price { get; set; }

		[ForeignKey("FacilityId")]
		public Facility Facility { get; set; }
	}
}
