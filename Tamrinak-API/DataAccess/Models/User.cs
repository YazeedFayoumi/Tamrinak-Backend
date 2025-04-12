using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DataAccess.Models
{
    public class User 
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public bool? IsEmaiConfirmed { get; set; } = false;
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ProfileImageUrl { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}
