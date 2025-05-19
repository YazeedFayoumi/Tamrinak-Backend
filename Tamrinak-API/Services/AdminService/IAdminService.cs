using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.PaginationDtos;
using Tamrinak_API.DTO.UserAuthDtos;

namespace Tamrinak_API.Services.AdminService
{
    public interface IAdminService 
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();
        Task<AdminUserDto> GetUserByIdAsync(int userId);
        //Task<bool> ChangeUserRoleAsync(int userId, string roleName);
        Task<List<AdminUserDto>> GetVenueOwnershipRequestsAsync();
        Task<bool> ApproveVenueOwnershipRequestAsync(int userId);
        Task<bool> RejectVenueOwnershipRequestAsync(int userId);
        Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);
        Task<bool> DeleteUserAsync(int userId);
        Task AddRoleToUserAsync(int userId, string roleName);
        Task RemoveRoleToUserAsync(int userId, string roleName);

        Task<List<AdminBookingDto>> GetUserBookingsAsync(int userId);
        Task<List<AdminMembershipDto>> GetUserMembershipsAsync(int userId);
        Task<FieldBookingStatsDto> GetFieldBookingStatsAsync(int fieldId);
        Task<FacilityMembershipStatsDto> GetFacilityMembershipStatsAsync(int facilityId);
        Task<object> GetVenueManagerOverviewsAsync();

        Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(bool? isVerified, int? facilityId, int? fieldId, int page, int pageSize);
        Task<AdminReviewDto?> GetReviewByIdAsync(int reviewId);
        Task VerifyReviewAsync(int reviewId);
        Task DeleteReviewAsync(int reviewId);
        Task<List<AdminReviewDto>> GetReviewRepliesAsync(int parentReviewId);
        Task ResetReviewLikesAsync(int reviewId);
        Task SetReviewLockStatusAsync(int reviewId, bool isLocked);
        Task BulkVerifyReviewsAsync(List<int> reviewIds);
        Task<PagedResult<AdminReviewDto>> GetReviewsByUserAsync(int userId, int page, int pageSize);

        Task<List<AdminPaymentDto>> GetAllPaymentsAsync(string? method, bool? confirmed);
        Task<AdminPaymentDto?> GetPaymentByIdAsync(int paymentId);
        Task ConfirmPaymentAsync(int paymentId);
        Task RefundPaymentAsync(int paymentId);
        Task<List<AdminPaymentDto>> GetPaymentsByUserAsync(int userId);

    }
}
