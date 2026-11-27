namespace Pharmacy.API.DTOs.Cart;

public class CartResponseDto
{
    public int CartId { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();

    public decimal TotalAmount { get; set; }
}

public class CartItemResponseDto
{
    public int CartItemId { get; set; }

    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }
}