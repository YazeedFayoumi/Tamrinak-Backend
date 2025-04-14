namespace Tamrinak_API.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? Name { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
