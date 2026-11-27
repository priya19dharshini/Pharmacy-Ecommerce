
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Data;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;

namespace Pharmacy_Backend.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly PharmacyDbContext _context;

    public CategoryRepository(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Where(c => c.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(
            category, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        _context.Categories.Update(category);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
