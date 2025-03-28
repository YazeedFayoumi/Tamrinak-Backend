using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DataAccess.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(30)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
