
using Pharmacy_Backend.DTOs.Medicine;

namespace Pharmacy_Backend.Services.Interfaces;

public interface IMedicineService
{
    Task<List<MedicineResponseDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<MedicineResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<MedicineResponseDto> CreateAsync(
        CreateMedicineDto dto,
        CancellationToken cancellationToken);

    Task<MedicineResponseDto?> UpdateAsync(
        int id,
        UpdateMedicineDto dto,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);

    Task<PagedMedicineResponseDto> SearchAsync(
        MedicineSearchDto dto,
        CancellationToken cancellationToken);

    Task<MedicineStockResponseDto?> UpdateStockAsync(
        int medicineId,
        UpdateStockDto dto,
        CancellationToken cancellationToken);

    Task<List<MedicineStockResponseDto>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken);

    Task<List<MedicineStockResponseDto>> GetOutOfStockAsync(
        CancellationToken cancellationToken);
}
