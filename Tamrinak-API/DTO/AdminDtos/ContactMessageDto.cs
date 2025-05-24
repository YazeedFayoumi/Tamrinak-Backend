using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.AdminDtos
{
    public class ContactMessageDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Email must be valid.")]
        public string Email { get; set; } 
        [MaxLength(1000, ErrorMessage = "Message is too long.")]
        public string Message { get; set; }
    }
}
