
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pharmacy_Backend.DTOs.Prescription
{
    public class UploadPrescriptionDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        public int? OrderId { get; set; }
    }
}
