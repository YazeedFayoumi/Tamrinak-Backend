using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.ReviewDtos
{
    public class AddReviewDto
    {
        public int? FacilityId { get; set; }
        public int? FieldId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
