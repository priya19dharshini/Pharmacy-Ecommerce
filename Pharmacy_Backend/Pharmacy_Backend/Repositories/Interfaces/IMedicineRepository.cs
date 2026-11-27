
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Repositories.Interfaces;

public interface IMedicineRepository
{
    Task<List<Medicine>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Medicine?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> CategoryExistsAsync(
        int categoryId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Medicine medicine,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Medicine medicine,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Medicine medicine,
        CancellationToken cancellationToken);

    Task<(List<Medicine> medicines, int totalCount)> SearchAsync(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? manufacturer,
        bool? requiresPrescription,
        bool? isActive,
        string sortBy,
        string sortOrder,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<List<Medicine>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken);

    Task<List<Medicine>> GetOutOfStockAsync(
        CancellationToken cancellationToken);
}
