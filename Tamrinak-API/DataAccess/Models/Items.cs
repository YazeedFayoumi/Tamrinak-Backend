namespace Tamrinak_API.DataAccess.Models
{
    public class Items
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

       
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }

}
