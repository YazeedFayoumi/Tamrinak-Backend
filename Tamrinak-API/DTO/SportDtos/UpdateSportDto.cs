using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.SportDtos
{
    public class UpdateSportDto
    {

        [MaxLength(25)]
        public string? Name { get; set; }
        [MaxLength(50)]
        public string? Description { get; set; }

        public List<string>? ImageUrls { get; set; }

    }
}
