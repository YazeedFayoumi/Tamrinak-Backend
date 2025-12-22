namespace Tamrinak_API.DTO.StoreDtos
{
    public class ItemListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public string? Image { get; set; }
    }

}
