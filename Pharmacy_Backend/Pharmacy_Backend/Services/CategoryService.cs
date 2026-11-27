
using Pharmacy_Backend.DTOs.Category;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllAsync(
            cancellationToken);

        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        return category == null ? null : MapToResponse(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var name = dto.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name, null, cancellationToken))
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        var category = new Category
        {
            Name = name,
            Description = dto.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
            category, cancellationToken);

        return MapToResponse(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category == null)
        {
            return null;
        }

        var name = dto.Name.Trim();

        if (await _repository.ExistsByNameAsync(
            name, id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Another category with this name already exists.");
        }

        category.Name = name;
        category.Description = dto.Description?.Trim();
        category.IsActive = dto.IsActive;

        await _repository.UpdateAsync(
            category, cancellationToken);

        return MapToResponse(category);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category == null)
        {
            return false;
        }

        await _repository.DeleteAsync(
            category, cancellationToken);

        return true;
    }

    private static CategoryResponseDto MapToResponse(
        Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}
