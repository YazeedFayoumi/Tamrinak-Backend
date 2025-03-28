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

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyFee { get; set; }

        public User User { get; set; }
        public Payment Payment { get; set; }
        public Facility Facility { get; set; }
    }
}
