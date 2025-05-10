using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.ReviewDtos
{
    public class UpdateReplyDto
    {
        [Required]
        public string Comment { get; set; }
    }
}
