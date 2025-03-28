using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
    public class Payment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }
        [Required] 
        public int MembershipId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentMethod { get; set; }  

        public string TransactionId { get; set; }

        public Booking Booking { get; set; }
        public Membership Membership { get; set; }
    }
    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        ClQ,
        Cash
    }
}
