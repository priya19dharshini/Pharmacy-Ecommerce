namespace Pharmacy.API.Models;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;

    public Medicine Medicine { get; set; } = null!;
}