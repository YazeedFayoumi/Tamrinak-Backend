namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminUpdateUserDto
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public bool? IsActive { get; set; }

    }
}
