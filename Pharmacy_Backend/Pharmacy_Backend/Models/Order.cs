namespace Pharmacy.API.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = "COD";

    public OrderStatus Status { get; set; }
        = OrderStatus.Placed;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}