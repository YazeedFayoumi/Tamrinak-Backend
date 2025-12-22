namespace Tamrinak_API.DTO.StoreDtos
{
    public class ItemDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<string> Images { get; set; } = new();
    }

}
