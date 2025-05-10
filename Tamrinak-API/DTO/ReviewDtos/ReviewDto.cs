namespace Tamrinak_API.DTO.ReviewDtos
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Likes { get; set; }
        public int? FacilityId { get; set; }
        public int? FieldId { get; set; }
        public bool IsVerified { get; set; }
        public int? ParentReviewId { get; set; }
    }
}
