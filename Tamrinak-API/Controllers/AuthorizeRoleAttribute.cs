using Microsoft.AspNetCore.Authorization;

namespace Tamrinak_API.Controllers
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
