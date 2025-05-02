using System.ComponentModel.DataAnnotations;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DTO.FieldDtos
{
    public class AddFieldDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LocationDesc { get; set; }
        [Required]
        public int SportId { get; set; }
        public decimal PricePerHour { get; set; }
        [Required]
        public TimeOnly OpenTime { get; set; }
        [Required]
        public TimeOnly CloseTime { get; set; }
        //[Phone, MinLength =10]
        [Required, Phone]
        public string? PhoneNumber { get; set; }
        public int? Capacity { get; set; }
        
        public string? LocationMap { get; set; }
        public bool? HasLighting { get; set; }
        public bool? IsIndoor { get; set; }
        
    }
}
