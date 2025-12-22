namespace Tamrinak_API.DataAccess.Models
{
    public class Orders
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }



        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public enum OrderStatus
    {
        PendingPayment = 1,
        Paid = 2,
        Cancelled = 3,
        Refunded = 4
    }


}
