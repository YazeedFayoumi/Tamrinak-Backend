namespace Tamrinak_API.DTO.AdminDtos
{
    public class AdminReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? FacilityName { get; set; }
        public string? FieldName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsVerified { get; set; }
        public int Likes { get; set; }
        public int? ParentReviewId { get; set; }
        public bool isLocked { get; set;}
        public bool IsReply => ParentReviewId.HasValue;
    }
}
