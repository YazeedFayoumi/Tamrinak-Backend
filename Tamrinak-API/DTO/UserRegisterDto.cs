using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email {  get; set; }
        public required string Password { get; set; }
        public string? Name { get; set; }

    }
}
