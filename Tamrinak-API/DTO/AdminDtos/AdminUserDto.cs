namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminUserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
        public bool HasPendingVenueRequest { get; set; }
        public DateTime? VenueRequestDate { get; set; }
    }
}
