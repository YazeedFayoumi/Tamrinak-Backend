namespace Tamrinak_API.DTO.StoreDtos
{
    public class CreateItemDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int? FacilityId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }

}
