using Tamrinak_API.DTO.SportDtos;

namespace Tamrinak_API.DTO.FieldDtos
{
    public class FieldBySportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationDesc { get; set; }
        public decimal? PricePerHour { get; set; }
        public SportBasicDto Sport { get; set; }
        public List<string>? Images { get; set; }
    }
}
