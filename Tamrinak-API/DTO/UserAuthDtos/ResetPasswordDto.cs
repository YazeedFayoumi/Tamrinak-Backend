using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.UserAuthDtos
{
	public class ResetPasswordDto
	{
		[Required]
		public string Token { get; set; }
		[Required]
		public string NewPassword { get; set; }
	}
}
