namespace Tamrinak_API.DTO.UserAuthDtos
{
	public class ProfileDto
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string? ProfileImageBase64 { get; set; }
		public string? Name { get; set; }
		public List<string> Roles { get; set; } = new();
	}
}
