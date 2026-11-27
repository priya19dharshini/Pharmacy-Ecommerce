using System.ComponentModel.DataAnnotations;

namespace Pharmacy.API.DTOs.Order;

public class CreateOrderDto
{
    [Required]
    [MaxLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required]
    public string PaymentMethod { get; set; } = "COD";

    public int? PrescriptionId { get; set; }
}