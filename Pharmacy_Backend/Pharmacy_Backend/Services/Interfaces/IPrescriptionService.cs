
using Microsoft.AspNetCore.Http;
using Pharmacy_Backend.DTOs.Prescription;

namespace Pharmacy_Backend.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> UploadAsync(
            string userId,
            IFormFile file,
            int? orderId);

        Task<List<PrescriptionResponseDto>> GetMyPrescriptionsAsync(
            string userId);

        Task<List<PrescriptionResponseDto>> GetAllAsync();

        Task<PrescriptionResponseDto?> GetByIdAsync(int id);

        Task<bool> ReviewAsync(
            int id,
            string reviewerId,
            ReviewPrescriptionDto dto);

        Task<(string FilePath, string ContentType)?> GetFileAsync(int id);
    }
}
