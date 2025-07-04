﻿using Tamrinak_API.DTO.SportDtos;

namespace Tamrinak_API.DTO.FacilityDtos
{
	public class FacilityBySportDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Type { get; set; } // e.g. "Gym", "Club"
		public decimal? PricePerMonth { get; set; }
		public List<SportBasicDto> Sports { get; set; }
		public List<string>? Images { get; set; }

		//TODO: more data 	e.g	public decimal? PricePerMonth { get; set; }
	}
}
