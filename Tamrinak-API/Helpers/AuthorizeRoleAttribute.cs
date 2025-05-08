using Microsoft.AspNetCore.Authorization;

namespace Tamrinak_API.Helpers
{
	public class AuthorizeRoleAttribute : AuthorizeAttribute
	{
		public AuthorizeRoleAttribute(params string[] roles)
		{
			Roles = string.Join(",", roles);
		}
	}
}
