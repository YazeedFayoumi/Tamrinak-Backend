namespace Tamrinak_API.DTO
{
    public class UserRegisterDto
    {
        public required string Email {  get; set; }
        public required string Password { get; set; }
        public string? Name { get; set; }
    }
}
