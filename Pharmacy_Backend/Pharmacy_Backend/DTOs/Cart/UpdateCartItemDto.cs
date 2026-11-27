using System.ComponentModel.DataAnnotations;

namespace Pharmacy.API.DTOs.Cart;

public class UpdateCartItemDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}
