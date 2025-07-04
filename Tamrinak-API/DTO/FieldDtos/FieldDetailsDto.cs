﻿using Tamrinak_API.DTO.SportDtos;

namespace Tamrinak_API.DTO.FieldDtos
{
	public class FieldDetailsDto
	{
		public int FieldId { get; set; }
		public string Name { get; set; }
		public string LocationDesc { get; set; }
		public string? LocationMap { get; set; }
		public string? PhoneNumber { get; set; }
		public int? Capacity { get; set; }
		public decimal PricePerHour { get; set; }
		public bool? HasLighting { get; set; }
		public bool? IsIndoor { get; set; }
		public string OpenTime { get; set; }
		public bool IsAvailable { get; set; }
		public string CloseTime { get; set; }
        public int? OwnerId { get; set; }

        public SportBasicDto Sport { get; set; }
	}

}
