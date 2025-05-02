using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.UserAuthDtos
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", //(?=.*[A-Z])
         ErrorMessage = "Password must include: number, and special character")]
        public required string Password { get; set; }

        public string? Name { get; set; }

    }
}
