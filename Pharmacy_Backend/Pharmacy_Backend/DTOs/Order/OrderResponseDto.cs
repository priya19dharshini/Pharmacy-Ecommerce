namespace Pharmacy.API.DTOs.Order;

public class OrderResponseDto
{
    public int OrderId { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }
}