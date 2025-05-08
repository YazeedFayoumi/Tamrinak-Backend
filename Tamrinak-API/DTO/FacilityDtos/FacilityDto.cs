using System.ComponentModel.DataAnnotations;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.FacilityDtos
{
	public class FacilityDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string LocationDesc { get; set; }
		public FacilityType Type { get; set; }
		public string OpenTime { get; set; }
		public string CloseTime { get; set; }

		public string? LocationMap { get; set; }
		public string? PhoneNumber { get; set; }
		public decimal? PricePerMonth { get; set; }
		public bool? IsAvailable { get; set; } = true;

		public string? Description { get; set; }
	}
}
