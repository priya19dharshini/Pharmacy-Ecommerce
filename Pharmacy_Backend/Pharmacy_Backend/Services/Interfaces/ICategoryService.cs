
using Pharmacy_Backend.DTOs.Category;

namespace Pharmacy_Backend.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CategoryResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken);

    Task<CategoryResponseDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}
