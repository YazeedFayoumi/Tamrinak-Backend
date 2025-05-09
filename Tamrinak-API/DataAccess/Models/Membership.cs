using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Membership
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int MembershipId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int FacilityId { get; set; }

		[Required, DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[Required, DataType(DataType.Date)]
		public DateTime ExpirationDate { get; set; }

		public bool IsActive { get; set; } = true;

		[Required, Column(TypeName = "decimal(10,2)")]
		public decimal MonthlyFee { get; set; }

        public decimal? TotalOfferPaid { get; set; } // total offer price paid upfront

        public User User { get; set; }
		public Payment Payment { get; set; }
		public Facility Facility { get; set; }
	}
}
