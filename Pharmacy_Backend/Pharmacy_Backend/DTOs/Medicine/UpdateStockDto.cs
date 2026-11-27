namespace Pharmacy_Backend.DTOs.Medicine;

/// <summary>
/// DTO for updating medicine stock quantity.
/// Supports three operations: Increase, Decrease, and Set.
/// </summary>
public class UpdateStockDto
{
    /// <summary>
    /// The quantity to update by or set to.
    /// - For Increase: must be greater than 0
    /// - For Decrease: must be greater than 0
    /// - For Set: can be 0 or greater
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The operation to perform: "Increase", "Decrease", or "Set".
    /// - Increase: Add quantity to current stock
    /// - Decrease: Subtract quantity from current stock
    /// - Set: Replace current stock with the specified quantity
    /// </summary>
    public string Operation { get; set; } = string.Empty;
}
