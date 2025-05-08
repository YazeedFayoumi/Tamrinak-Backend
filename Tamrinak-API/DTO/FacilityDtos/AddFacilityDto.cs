using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Helpers;

namespace Tamrinak_API.DTO.FacilityDtos
{
	public class AddFacilityDto
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string LocationDesc { get; set; }
		public FacilityType Type { get; set; }
		public List<int> SportIds { get; set; }

		[Required]
		public string OpenTime { get; set; }

		[Required]
		public string CloseTime { get; set; }

		public string? LocationMap { get; set; }
		[Phone]
		public string? PhoneNumber { get; set; }
		public string? Description { get; set; }
		public decimal? PricePerMonth { get; set; }
		public int? OfferDurationInMonths { get; set; }
		public decimal? OfferPrice { get; set; }
		public bool? IsAvailable { get; set; } = true;


	}
}
