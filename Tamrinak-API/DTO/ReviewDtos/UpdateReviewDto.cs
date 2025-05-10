using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.ReviewDtos
{
    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
