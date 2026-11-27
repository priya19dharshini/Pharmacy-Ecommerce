
using Pharmacy_Backend.DTOs.Prescription;
using Pharmacy_Backend.Helpers;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _repository;
        private readonly FileUploadHelper _fileHelper;

        public PrescriptionService(
            IPrescriptionRepository repository,
            FileUploadHelper fileHelper)
        {
            _repository = repository;
            _fileHelper = fileHelper;
        }

        public async Task<PrescriptionResponseDto> UploadAsync(
            string userId,
            IFormFile file,
            int? orderId)
        {
            var savedFile = await _fileHelper.SaveFileAsync(file);

            try
            {
                var prescription = new Prescription
                {
                    UserId = userId,
                    OrderId = orderId,
                    FileName = savedFile.FileName,
                    FilePath = savedFile.StoredFileName,
                    UploadedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                var result = await _repository.AddAsync(prescription);

                return MapToDto(result);
            }
            catch
            {
                _fileHelper.DeleteFile(savedFile.StoredFileName);
                throw;
            }
        }

        public async Task<List<PrescriptionResponseDto>>
            GetMyPrescriptionsAsync(string userId)
        {
            var items = await _repository.GetByUserIdAsync(userId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<PrescriptionResponseDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<PrescriptionResponseDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<bool> ReviewAsync(
            int id,
            string reviewerId,
            ReviewPrescriptionDto dto)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
                return false;

            string status = dto.Status.Trim();

            if (status != "Approved" && status != "Rejected")
                throw new ArgumentException(
                    "Status must be Approved or Rejected."
                );

            if (status == "Rejected" &&
                string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                throw new ArgumentException(
                    "Rejection reason is required."
                );
            }

            if (item.Status != "Pending")
                throw new ArgumentException(
                    "Only pending prescriptions can be reviewed."
                );

            item.Status = status;
            item.ReviewedBy = reviewerId;
            item.ReviewedAt = DateTime.UtcNow;
            item.RejectionReason = status == "Rejected"
                ? dto.RejectionReason!.Trim()
                : null;

            await _repository.UpdateAsync(item);

            return true;
        }

        public async Task<(string FilePath, string ContentType)?>
            GetFileAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
                return null;

            string fullPath = _fileHelper.GetFilePath(item.FilePath);

            if (!File.Exists(fullPath))
                return null;

            string extension =
                Path.GetExtension(item.FilePath).ToLowerInvariant();

            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return (fullPath, contentType);
        }

        private static PrescriptionResponseDto MapToDto(
            Prescription item)
        {
            return new PrescriptionResponseDto
            {
                Id = item.Id,
                UserId = item.UserId,
                OrderId = item.OrderId,
                FileName = item.FileName,
                FileUrl = $"/api/prescriptions/{item.Id}/file",
                UploadedAt = item.UploadedAt,
                Status = item.Status,
                ReviewedBy = item.ReviewedBy,
                ReviewedAt = item.ReviewedAt,
                RejectionReason = item.RejectionReason
            };
        }
    }
}
