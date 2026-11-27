
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_Backend.DTOs.Medicine;

public class CreateMedicineDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool RequiresPrescription { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(150)]
    public string Manufacturer { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
