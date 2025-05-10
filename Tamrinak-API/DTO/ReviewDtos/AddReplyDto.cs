using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.ReviewDtos
{
    public class AddReplyDto
    {
        public int ParentReviewId { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
