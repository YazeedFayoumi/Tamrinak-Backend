using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.BookingDtos
{
	public class AvailabilityDto
	{
		[DataType(DataType.Date)]
		public DateTime BookingDate { get; set; }
		public TimeOnly Start { get; set; }
		public TimeOnly End { get; set; }
	}
}
