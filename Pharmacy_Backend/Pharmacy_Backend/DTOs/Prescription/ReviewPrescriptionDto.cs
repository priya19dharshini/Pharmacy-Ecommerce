
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_Backend.DTOs.Prescription
{
    public class ReviewPrescriptionDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }
}
