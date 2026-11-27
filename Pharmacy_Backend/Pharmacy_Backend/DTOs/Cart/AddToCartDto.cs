using System.ComponentModel.DataAnnotations;

namespace Pharmacy.API.DTOs.Cart;

public class AddToCartDto
{
    [Required]
    public int MedicineId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}