using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.FieldDtos
{
	public class FieldDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string LocationDesc { get; set; }
		public int SportId { get; set; }
		public string OpenTime { get; set; }
		public string CloseTime { get; set; }

		public string? PhoneNumber { get; set; }

		public int? Capacity { get; set; }

		public string? LocationMap { get; set; }
		public bool? HasLighting { get; set; }
		public bool? IsIndoor { get; set; }
		public decimal? PricePerHour { get; set; }
	}
}
