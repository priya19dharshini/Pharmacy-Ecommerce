
namespace Pharmacy_Backend.DTOs.Prescription
{
    public class PrescriptionResponseDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileUrl { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public string? RejectionReason { get; set; }
    }
}
