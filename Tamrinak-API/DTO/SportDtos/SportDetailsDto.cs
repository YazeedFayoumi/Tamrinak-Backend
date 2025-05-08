namespace Tamrinak_API.DTO.SportDtos
{
	public class SportDetailsDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public List<string> ImageUrls { get; set; } = new();
	}
}
