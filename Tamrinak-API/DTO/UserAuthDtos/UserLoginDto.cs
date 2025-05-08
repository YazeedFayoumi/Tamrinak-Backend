using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.UserAuthDtos
{
	public class UserLoginDto
	{
		[Required]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
