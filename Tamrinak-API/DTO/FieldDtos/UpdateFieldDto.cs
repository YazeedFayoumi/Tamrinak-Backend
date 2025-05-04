using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.FieldDtos
{
    public class UpdateFieldDto
    {
        [Required]
        public string Name { get; set; }

        public string? LocationMap { get; set; }

        [Required]
        public string LocationDesc { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public int? Capacity { get; set; }

        public decimal PricePerHour { get; set; }

        public bool? HasLighting { get; set; }

        public bool? IsIndoor { get; set; }

        [Required]
        public TimeOnly OpenTime { get; set; }

        [Required]
        public TimeOnly CloseTime { get; set; }

        public int SportId { get; set; }

    }
}
