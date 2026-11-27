namespace Pharmacy_Backend.DTOs.Medicine;

/// <summary>
/// DTO for medicine stock response.
/// Includes the stock quantity and calculated stock status.
/// </summary>
public class MedicineStockResponseDto
{
    /// <summary>
    /// The unique identifier of the medicine.
    /// </summary>
    public int MedicineId { get; set; }

    /// <summary>
    /// The name of the medicine.
    /// </summary>
    public string MedicineName { get; set; } = string.Empty;

    /// <summary>
    /// The current stock quantity.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// The stock status based on quantity.
    /// Values: "OutOfStock" (qty = 0), "LowStock" (1-10), "InStock" (>10).
    /// </summary>
    public string StockStatus { get; set; } = string.Empty;
}
