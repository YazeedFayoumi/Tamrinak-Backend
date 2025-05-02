using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.Services.MembershipService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        //TODO
    }
}
