namespace Pharmacy_Backend.DTOs.Medicine;

/// <summary>
/// DTO for searching and filtering medicines with pagination support.
/// All filter properties are optional (nullable) to allow flexible filtering.
/// </summary>
public class MedicineSearchDto
{
    /// <summary>
    /// Search medicines by name (case-insensitive, contains match).
    /// Optional - leave null to skip search by name.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter medicines by category ID.
    /// Optional - leave null to include all categories.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter medicines with minimum price (inclusive).
    /// Optional - leave null for no minimum price filter.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Filter medicines with maximum price (inclusive).
    /// Optional - leave null for no maximum price filter.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Filter medicines by manufacturer name.
    /// Optional - leave null to include all manufacturers.
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Filter medicines that require prescription.
    /// Optional - leave null to include all medicines regardless of prescription requirement.
    /// </summary>
    public bool? RequiresPrescription { get; set; }

    /// <summary>
    /// Filter medicines by active/inactive status.
    /// Optional - leave null to include all medicines.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Field to sort results by: "name", "price", or "createdAt".
    /// Default: "name"
    /// </summary>
    public string SortBy { get; set; } = "name";

    /// <summary>
    /// Sort direction: "asc" for ascending or "desc" for descending.
    /// Default: "asc"
    /// </summary>
    public string SortOrder { get; set; } = "asc";

    /// <summary>
    /// Page number for pagination (1-based).
    /// Default: 1
    /// Minimum: 1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of records per page.
    /// Default: 10
    /// Maximum: 100
    /// </summary>
    public int PageSize { get; set; } = 10;
}
