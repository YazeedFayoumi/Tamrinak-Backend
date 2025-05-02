using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.MembershipService
{
    public class MembershipService : IMembershipService
    {
        private readonly IGenericRepo<Membership> _membershipRepo;
        private readonly IGenericRepo<Facility> _facilityRepo;
        public MembershipService(IGenericRepo<Membership> membershipRepo, IGenericRepo<Facility> facilityRepo)
        {
            _membershipRepo = membershipRepo;
            _facilityRepo = facilityRepo;
        }


    }
}
