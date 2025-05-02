using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.SportDtos
{
    public class AddSportDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } 
    }
}
