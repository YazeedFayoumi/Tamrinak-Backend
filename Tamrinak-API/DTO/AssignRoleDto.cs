namespace Tamrinak_API.DTO
{
    public class AssignRoleDto
    {
        public required string UserEmail { get; set; }
        public required List<string> RoleNames { get; set; }
    }

}
