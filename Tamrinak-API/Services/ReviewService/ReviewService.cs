using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.ReviewDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IGenericRepo<Review> _reviewRepo;
        private readonly IGenericRepo<User> _userRepo;
        private readonly IGenericRepo<Field> _fieldRepo;
        private readonly IGenericRepo<Facility> _facilityRepo;
        private readonly IGenericRepo<Booking> _bookingRepo;
        private readonly IGenericRepo<Membership> _membershipRepo;
        public ReviewService(IGenericRepo<Review> reviewRepo,
                             IGenericRepo<User> userRepo,
                             IGenericRepo<Field> fieldRepo,
                             IGenericRepo<Facility> facilityRepo,
                             IGenericRepo<Booking> bookingRepo, IGenericRepo<Membership> memRepo)
        {
            _reviewRepo = reviewRepo;
            _userRepo = userRepo;
            _fieldRepo = fieldRepo;
            _facilityRepo = facilityRepo;
            _bookingRepo = bookingRepo;
            _membershipRepo = memRepo;
        }

        public async Task<ReviewDto> AddReviewAsync(AddReviewDto dto, string userEmail)
        {
            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
                       ?? throw new Exception("User not found");
            if (!user.IsEmailConfirmed)
            {
                throw new Exception("Email must be confirmed to review");
            }

            bool isVerified = false;

            if (dto.FieldId.HasValue)
            {
                isVerified = await _bookingRepo.ExistsAsync(b =>
                    b.UserId == user.UserId &&
                    b.FieldId == dto.FieldId &&
                    b.BookingDate <= DateTime.UtcNow
                );
            }
            else if (dto.FacilityId.HasValue)
            {
                isVerified = await _membershipRepo.ExistsAsync(m =>
                    m.UserId == user.UserId &&
                    m.FacilityId == dto.FacilityId &&
                    m.StartDate <= DateTime.UtcNow
                );
            }

            var review = new Review
            {
                UserId = user.UserId,
                FieldId = dto.FieldId,
                FacilityId = dto.FacilityId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow,
                IsVerified = isVerified
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveAsync();

            return ToDto(review, user.Name);
        }

        public async Task<List<ReviewDto>> GetReviewsForFieldAsync(int fieldId)
        {
            var reviews = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.FieldId == fieldId,
                include: q => q.Include(r => r.User)
            );

            return reviews.Select(r => ToDto(r, r.User.Name)).ToList();
        }

        public async Task<List<ReviewDto>> GetReviewsForFacilityAsync(int facilityId)
        {
            var reviews = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.FacilityId == facilityId,
                include: q => q.Include(r => r.User)
            );

            return reviews.Select(r => ToDto(r, r.User.Name)).ToList();
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepo.GetByConditionIncludeAsync(
                r => r.ReviewId == reviewId,
                include: q => q.Include(r => r.User)
            );

            if (review == null)
                throw new Exception("Review not found");

            return ToDto(review, review.User.Name);
        }

        public async Task<List<ReviewDto>> GetUserReviewsAsync(string userEmail)
        {
            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
                       ?? throw new Exception("User not found");

            var reviews = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.UserId == user.UserId,
                include: q => q.Include(r => r.Facility).Include(r => r.Field)
            );

            return reviews.Select(r => new ReviewDto
            {
                UserId = user.UserId,
                ReviewId = r.ReviewId,
                UserName = user.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                Likes = r.Likes,
                FacilityId = r.FacilityId,
                FieldId = r.FieldId,
                
            }).ToList();
        }

        public async Task<ReviewDto> UpdateReviewAsync(int reviewId, UpdateReviewDto dto, string userEmail)
        {
            var review = await _reviewRepo.GetByConditionIncludeAsync(
                r => r.ReviewId == reviewId,
                include: q => q.Include(r => r.User)
            );

            if (review == null)
                throw new Exception("Review not found");

            if (review.User.Email != userEmail)
                throw new UnauthorizedAccessException("You can only update your own reviews");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveAsync();

            return ToDto(review, review.User.Name);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userEmail)
        {
            var review = await _reviewRepo.GetByConditionIncludeAsync(
                r => r.ReviewId == reviewId,
                include: q => q.Include(r => r.User)
            );

            if (review == null || review.User.Email != userEmail)
                return false;

            await _reviewRepo.DeleteAsync(review);
            await _reviewRepo.SaveAsync();
            return true;
        }

        public async Task<bool> LikeReviewAsync(int reviewId)
        {
            var review = await _reviewRepo.GetAsync(reviewId);
            if (review == null) return false;

            review.Likes += 1;
            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveAsync();
            return true;
        }

        public async Task<ReviewDto> AddReplyAsync(AddReplyDto dto, string userEmail)
        {
            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
                       ?? throw new Exception("User not found");

            var parent = await _reviewRepo.GetAsync(dto.ParentReviewId)
                         ?? throw new Exception("Parent review not found");

            if (parent.IsLocked)
                throw new Exception("Replies are not allowed for this review.");

            var reply = new Review
            {
                UserId = user.UserId,
                ParentReviewId = dto.ParentReviewId,
                Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow,
                Rating = 0, // or null if allowed
                IsVerified = false,
                FacilityId = null,
                FieldId = null
            };

            await _reviewRepo.AddAsync(reply);
            await _reviewRepo.SaveAsync();

            return new ReviewDto
            {
                UserId = user.UserId,
                ReviewId = reply.ReviewId,
                UserName = user.Name,
                Rating = reply.Rating,
                Comment = reply.Comment,
                ReviewDate = reply.ReviewDate,
                Likes = 0,
                FacilityId = null,
                FieldId = null,
                ParentReviewId = reply.ParentReviewId,
            };
        }

        public async Task<List<ReviewDto>> GetRepliesAsync(int parentReviewId)
        {
            var replies = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.ParentReviewId == parentReviewId,
                include: q => q.Include(r => r.User)
            );

            return replies.Select(r => new ReviewDto
            {
                UserId = r.User.UserId,
                ReviewId = r.ReviewId,
                UserName = r.User.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                Likes = r.Likes,
                FacilityId = r.FacilityId,
                FieldId = r.FieldId,
                ParentReviewId =r.ParentReviewId,
            }).ToList();
        }

        public async Task<bool> UpdateReplyAsync(int replyId, string userEmail, UpdateReplyDto dto)
        {
            var reply = await _reviewRepo.GetAsync(replyId)
                ?? throw new Exception("Reply not found");

            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
                ?? throw new Exception("User not found");

            if (reply.UserId != user.UserId)
                throw new UnauthorizedAccessException("You are not allowed to update this reply.");

            if (reply.ParentReviewId == null)
                throw new Exception("This is not a reply.");

            reply.Comment = dto.Comment;
            reply.ReviewDate = DateTime.UtcNow;

            await _reviewRepo.UpdateAsync(reply);
            await _reviewRepo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteReplyAsync(int replyId, string userEmail)
        {
            var reply = await _reviewRepo.GetAsync(replyId)
                ?? throw new Exception("Reply not found");

            var user = await _userRepo.GetByConditionAsync(u => u.Email == userEmail)
                ?? throw new Exception("User not found");

            if (reply.UserId != user.UserId)
                throw new UnauthorizedAccessException("You are not allowed to delete this reply.");

            if (reply.ParentReviewId == null)
                throw new Exception("This is not a reply.");

            var childReplies = await _reviewRepo.GetListByConditionAsync(r => r.ParentReviewId == replyId);
            foreach (var rreply in childReplies)
                await _reviewRepo.DeleteAsync(rreply);

            await _reviewRepo.DeleteAsync(reply);
            await _reviewRepo.SaveAsync();
            return true;
        }

        private static ReviewDto ToDto(Review r, string username) => new ReviewDto
        {
            ReviewId = r.ReviewId,
            UserId = r.UserId,
            UserName = username,
            Rating = r.Rating,
            Comment = r.Comment,
            ReviewDate = r.ReviewDate,
            Likes = r.Likes,
            FacilityId = r.FacilityId,
            FieldId = r.FieldId,
            IsVerified = r.IsVerified
        };

    }
}
