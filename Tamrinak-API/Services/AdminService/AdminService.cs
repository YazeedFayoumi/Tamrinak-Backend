using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.AdminDtos;
using Tamrinak_API.DTO.PaginationDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepo<User> _userRepo;
        private readonly IGenericRepo<UserRole> _userRoleRepo;
        private readonly IGenericRepo<Role> _roleRepo;

        private readonly IGenericRepo<Booking> _bookingRepo;
        private readonly IGenericRepo<Membership> _membershipRepo;

        private readonly IGenericRepo<Review> _reviewRepo;
        private readonly IGenericRepo<Payment> _paymentRepo;
        public AdminService(IGenericRepo<User> userRepo, IGenericRepo<UserRole> userRoleRepo, IGenericRepo<Role> roleRepo,
            IGenericRepo<Booking> bookingRepo, IGenericRepo<Membership> membershipRepo, 
            IGenericRepo<Review> reviewRepo, IGenericRepo<Payment> paymentRepo)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _roleRepo = roleRepo;
            _bookingRepo = bookingRepo;
            _membershipRepo = membershipRepo;
            _reviewRepo = reviewRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetListByConditionIncludeAsync(
     predicate: u => true, // or any real filter if needed
     include: query => query
         .Include(u => u.UserRoles)
         .ThenInclude(ur => ur.Role)
 );


            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = u.HasVenueOwnershipRequest,
                VenueRequestDate = u.VenueRequestDate
            }).ToList();
        }

        public async Task<AdminUserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepo.GetByConditionIncludeAsync(
                u => u.UserId == userId,
                include: u => u.Include(x => x.UserRoles).ThenInclude(ur => ur.Role));

            if (user == null)
                throw new Exception("User not found");

            return new AdminUserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = user.HasVenueOwnershipRequest,
                VenueRequestDate = user.VenueRequestDate
            };
        }

        public async Task AddRoleToUserAsync(int userId, string roleName)
        {
            var user = await _userRepo.GetByConditionAsync(u => u.UserId == userId);
            if (user == null) throw new Exception("User not found");

            var role = await _roleRepo.GetByConditionAsync(r => r.RoleName == roleName);
            if (role == null) throw new Exception("Role not found");

            var exists = await _userRoleRepo.GetByConditionAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);
            if (exists != null) throw new Exception("User already has this role");

            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = role.RoleId
            });
            await _userRoleRepo.SaveAsync();
        }

        public async Task RemoveRoleToUserAsync(int userId, string roleName)
        {
            var role = await _roleRepo.GetByConditionAsync(r => r.RoleName == roleName);
            if (role == null) throw new Exception("Role not found");

            var userRole = await _userRoleRepo.GetByConditionAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);
            if (userRole == null) throw new Exception("Role not assigned to user");

            await _userRoleRepo.DeleteAsync(userRole);
            await _userRoleRepo.SaveAsync();
        }

        public async Task<List<AdminUserDto>> GetVenueOwnershipRequestsAsync()
        {
            var users = await _userRepo.GetListByConditionAsync(u => u.HasVenueOwnershipRequest == true);
            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                HasPendingVenueRequest = u.HasVenueOwnershipRequest,
                VenueRequestDate = u.VenueRequestDate
            }).ToList();
        }

        public async Task<bool> ApproveVenueOwnershipRequestAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");

            user.HasVenueOwnershipRequest = false;
            user.VenueRequestDate = null;

            var venueManagerRole = await _roleRepo.GetByConditionAsync(r => r.RoleName == "VenueManager");
            if (venueManagerRole == null)
                throw new Exception("VenueManager role not found");

            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = venueManagerRole.RoleId
            });

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> RejectVenueOwnershipRequestAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");

            user.HasVenueOwnershipRequest = false;
            user.VenueRequestDate = null;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");
            user.IsActive = isActive;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetAsync(userId) ?? throw new Exception("User not found");
            await _userRepo.DeleteAsync(user);
            await _userRepo.SaveAsync();
            return true;
        }

        public async Task<List<AdminBookingDto>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _bookingRepo.GetListByConditionIncludeAsync(
                predicate: b => b.UserId == userId,
                include: q => q.Include(b => b.Field));
                              

            return bookings.Select(b => new AdminBookingDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                UserName = b.User?.Name,
                FieldName = b.Field?.Name,
                FacilityName = b.FacilityId != null ? b.SportFacility?.Facility?.Name : null,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime.ToString("HH:mm"), // return as string
                EndTime = b.EndTime.ToString("HH:mm"),     // return as string
                Duration = b.Duration,
                NumberOfPeople = b.NumberOfPeople,
                TotalCost = b.TotalCost,
                IsPaid = b.IsPaid,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                CancelledAt = b.CancelledAt

            }).ToList();
        }

        public async Task<List<AdminMembershipDto>> GetUserMembershipsAsync(int userId)
        {
            var memberships = await _membershipRepo.GetListByConditionIncludeAsync(
                predicate: m => m.UserId == userId,
                include: q => q.Include(m => m.Facility));

            return memberships.Select(m => new AdminMembershipDto
            {
                MembershipId = m.MembershipId,
                UserId = m.UserId,
                UserName = m.User?.Name,
                FacilityName = m.Facility?.Name,
                StartDate = m.StartDate,
                ExpirationDate = m.ExpirationDate,
                IsActive = m.IsActive,
                MonthlyFee = m.MonthlyFee,
                TotalOfferPaid = m.TotalOfferPaid
            }).ToList();
        }

        //reviews
        public async Task<AdminReviewDto?> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepo.GetByConditionIncludeAsync(
                r => r.ReviewId == reviewId,
                q => q.Include(r => r.User)
                      .Include(r => r.Facility)
                      .Include(r => r.Field)
            );

            return review != null ? ToDto(review) : null;
        }

        public async Task VerifyReviewAsync(int reviewId)
        {
            var review = await _reviewRepo.GetAsync(reviewId) ?? throw new Exception("Review not found");
            review.IsVerified = true;
            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepo.GetAsync(reviewId) ?? throw new Exception("Review not found");
            await _reviewRepo.DeleteAsync(review);
            await _reviewRepo.SaveAsync();
        }

        public async Task<List<AdminReviewDto>> GetReviewRepliesAsync(int parentReviewId)
        {
            var replies = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.ParentReviewId == parentReviewId,
                q => q.Include(r => r.User)
                      .Include(r => r.Facility)
                      .Include(r => r.Field)
            );

            return replies.Select(ToDto).ToList();
        }

        public async Task ResetReviewLikesAsync(int reviewId)
        {
            var review = await _reviewRepo.GetAsync(reviewId) ?? throw new Exception("Review not found");
            review.Likes = 0;
            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveAsync();
        }

        public async  Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(bool? isVerified, int? facilityId, int? fieldId, int page, int pageSize)
        {
            var allReviews = await _reviewRepo.GetListByConditionIncludeAsync(
             predicate: r =>
                 (!isVerified.HasValue || r.IsVerified == isVerified.Value) &&
                 (!facilityId.HasValue || r.FacilityId == facilityId.Value) &&
                 (!fieldId.HasValue || r.FieldId == fieldId.Value),
             include: q => q.Include(r => r.User)
                            .Include(r => r.Facility)
                            .Include(r => r.Field)
         );

            var paged = allReviews
                .OrderByDescending(r => r.ReviewDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return new PagedResult<AdminReviewDto>
            {
                Items = paged,
                TotalCount = allReviews.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task SetReviewLockStatusAsync(int reviewId, bool isLocked)
        {
            var review = await _reviewRepo.GetAsync(reviewId) ?? throw new Exception("Review not found");
            review.IsLocked = isLocked;
            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveAsync();
        }

        public async Task BulkVerifyReviewsAsync(List<int> reviewIds)
        {
            var reviews = await _reviewRepo.GetListByConditionAsync(r => reviewIds.Contains(r.ReviewId));

            foreach (var r in reviews)
            {
                r.IsVerified = true;
                await _reviewRepo.UpdateAsync(r);
            }
            await _reviewRepo.SaveAsync();
        }

        public async Task<PagedResult<AdminReviewDto>> GetReviewsByUserAsync(int userId, int page, int pageSize)
        {
            var reviews = await _reviewRepo.GetListByConditionIncludeAsync(
                r => r.UserId == userId,
                q => q.Include(r => r.Facility)
                      .Include(r => r.Field)
            );

            var ordered = reviews.OrderByDescending(r => r.ReviewDate).ToList();
            var paged = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<AdminReviewDto>
            {
                Items = paged.Select(ToDto).ToList(),
                TotalCount = reviews.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        private AdminReviewDto ToDto(Review r)
        {
            return new AdminReviewDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserName = r.User?.Name,
                FacilityName = r.Facility?.Name,
                FieldName = r.Field?.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                IsVerified = r.IsVerified,
                Likes = r.Likes,
                ParentReviewId = r.ParentReviewId,
                isLocked = r.IsLocked
            };
        }
        //payment
        public async Task<List<AdminPaymentDto>> GetAllPaymentsAsync(string? method, bool? confirmed)
        {
            var payments = await _paymentRepo.GetListByConditionIncludeAsync(
                p => (!confirmed.HasValue || p.IsConfirmed == confirmed.Value) &&
                     (string.IsNullOrEmpty(method) || p.Method.ToString() == method),
                q => q.Include(p => p.Booking).ThenInclude(b => b.User)
                      .Include(p => p.Membership).ThenInclude(m => m.User)
            );

            return payments.Select(ToDto).ToList();
        }

        public async Task<AdminPaymentDto?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetByConditionIncludeAsync(
                p => p.PaymentId == paymentId,
                q => q.Include(p => p.Booking).ThenInclude(b => b.User)
                      .Include(p => p.Membership).ThenInclude(m => m.User)
            );

            return payment != null ? ToDto(payment) : null;
        }

        public async Task ConfirmPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetAsync(paymentId) ?? throw new Exception("Payment not found");

            payment.IsConfirmed = true;
            payment.Status = PaymentStatus.Confirmed;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveAsync();
        }

        public async Task RefundPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetAsync(paymentId) ?? throw new Exception("Payment not found");

            payment.IsRefunded = true;
            payment.RefundedAt = DateTime.UtcNow;
            payment.Status = PaymentStatus.Refunded;

            await _paymentRepo.UpdateAsync(payment);
            await _paymentRepo.SaveAsync();
        }

        public async Task<List<AdminPaymentDto>> GetPaymentsByUserAsync(int userId)
        {
            var payments = await _paymentRepo.GetListByConditionIncludeAsync(
                p => (p.Booking != null && p.Booking.UserId == userId) || (p.Membership != null && p.Membership.UserId == userId),
                q => q.Include(p => p.Booking).ThenInclude(b => b.User)
                      .Include(p => p.Membership).ThenInclude(m => m.User)
            );

            return payments.Select(ToDto).ToList();
        }

        private AdminPaymentDto ToDto(Payment p)
        {
            var user = p.Booking?.User ?? p.Membership?.User;
            return new AdminPaymentDto
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                MembershipId = p.MembershipId,
                UserId = user?.UserId ?? 0,
                UserName = user?.Name ?? "N/A",
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.Method.ToString(),
                Status = p.Status.ToString(),
                TransactionId = p.TransactionId,
                IsConfirmed = p.IsConfirmed,
                IsRefunded = p.IsRefunded
            };
        }


    }
}
