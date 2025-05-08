using System.ComponentModel.DataAnnotations;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.FacilityDtos
{
	public class UpdateFacilityDto
	{
		[Required]
		public string Name { get; set; }

		public string? LocationMap { get; set; }

		[Required]
		public string LocationDesc { get; set; }

		[Phone]
		public string? PhoneNumber { get; set; }

		public decimal? PricePerMonth { get; set; }

		public bool? IsAvailable { get; set; }

		[Required]
		public string OpenTime { get; set; }

		[Required]
		public string CloseTime { get; set; }

		public string? Description { get; set; }

		[Required]
		public FacilityType Type { get; set; }

		public List<int>? SportIds { get; set; }
	}
}
