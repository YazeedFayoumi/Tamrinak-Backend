namespace Tamrinak_API.DTO.UserAuthDtos
{
    public class AssignRoleDto
    {
        public required string UserEmail { get; set; }
        public required List<string> RoleNames { get; set; }
    }

}
