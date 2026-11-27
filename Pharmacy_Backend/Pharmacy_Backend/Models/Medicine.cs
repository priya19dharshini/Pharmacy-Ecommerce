
namespace Pharmacy_Backend.Models;

public class Medicine
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public bool RequiresPrescription { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public string Manufacturer { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
