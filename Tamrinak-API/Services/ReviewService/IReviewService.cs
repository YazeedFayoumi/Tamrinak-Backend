using Tamrinak_API.DTO.ReviewDtos;

namespace Tamrinak_API.Services.ReviewService
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(AddReviewDto dto, string userEmail);
        Task<List<ReviewDto>> GetReviewsForFieldAsync(int fieldId);
        Task<List<ReviewDto>> GetReviewsForFacilityAsync(int facilityId);
        Task<ReviewDto> GetReviewByIdAsync(int reviewId);
        Task<bool> DeleteReviewAsync(int reviewId, string userEmail);
        Task<ReviewDto> UpdateReviewAsync(int reviewId, UpdateReviewDto dto, string userEmail);
        Task<bool> LikeReviewAsync(int reviewId);
        Task<List<ReviewDto>> GetUserReviewsAsync(string userEmail);

        Task<ReviewDto> AddReplyAsync(AddReplyDto dto, string userEmail);
        Task<List<ReviewDto>> GetRepliesAsync(int parentReviewId);
        Task<bool> UpdateReplyAsync(int replyId, string userEmail, UpdateReplyDto dto);
        Task<bool> DeleteReplyAsync(int replyId, string userEmail);

    }
}
