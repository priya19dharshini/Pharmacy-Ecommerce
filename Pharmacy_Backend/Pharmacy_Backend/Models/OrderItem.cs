namespace Pharmacy.API.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    // Store the price at the time of purchase.
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;

    public Medicine Medicine { get; set; } = null!;
}