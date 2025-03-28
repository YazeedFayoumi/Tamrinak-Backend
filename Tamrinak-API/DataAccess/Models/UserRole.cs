namespace Tamrinak_API.DataAccess.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
