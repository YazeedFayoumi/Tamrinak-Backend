using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.SportDtos;

namespace Tamrinak_API.DTO.FacilityDtos
{
	public class FacilityDetailsDto
	{
		public int FacilityId { get; set; }
		public string Name { get; set; }
		public string LocationDesc { get; set; }
		public string? LocationMap { get; set; }
		public string? PhoneNumber { get; set; }
		public decimal? PricePerMonth { get; set; }
		public bool? IsAvailable { get; set; }
		public string OpenTime { get; set; }
		public string CloseTime { get; set; }
		public string? Description { get; set; }
		public FacilityType Type { get; set; }
		public double AverageRating { get; set; }
		public int TotalReviews { get; set; }
		public int? OwnerId { get; set; }
		public List<SportBasicDto> Sports { get; set; }
	}

}
