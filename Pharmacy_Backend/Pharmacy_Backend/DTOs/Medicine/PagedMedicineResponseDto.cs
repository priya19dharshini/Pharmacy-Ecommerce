namespace Pharmacy_Backend.DTOs.Medicine;

/// <summary>
/// DTO for paginated medicine search results.
/// Contains the list of medicines for the current page along with pagination metadata.
/// </summary>
public class PagedMedicineResponseDto
{
    /// <summary>
    /// List of medicines for the current page.
    /// </summary>
    public List<MedicineResponseDto> Data { get; set; } = new();

    /// <summary>
    /// Total number of medicines matching the search/filter criteria (across all pages).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of records per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages available based on TotalCount and PageSize.
    /// Calculated as: Ceiling(TotalCount / PageSize)
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
