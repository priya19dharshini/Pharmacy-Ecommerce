
namespace Pharmacy.Domain.Entities;

public class Prescription
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string? ReviewRemarks { get; set; }

    public string? ReviewedBy { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }
}
