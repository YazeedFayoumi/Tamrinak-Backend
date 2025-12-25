using Stripe.Climate;

namespace Tamrinak_API.DataAccess.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Orders Order { get; set; } = null!;

        public int ItemId { get; set; }
        public Items Item { get; set; } = null!;

        public int Quantity { get; set; }

        // IMPORTANT: price at time of purchase
        public decimal UnitPrice { get; set; }
    }

}
