
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Category category,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken);
}
