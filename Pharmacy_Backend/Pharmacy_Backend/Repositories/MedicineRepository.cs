
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Data;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;

namespace Pharmacy_Backend.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly PharmacyDbContext _context;

    public MedicineRepository(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medicine>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Medicines
            .AsNoTracking()
            .Include(m => m.Category)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Medicine?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Medicines
            .Include(m => m.Category)
            .FirstOrDefaultAsync(
                m => m.Id == id,
                cancellationToken);
    }

    public async Task<bool> CategoryExistsAsync(
        int categoryId,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(
                c => c.Id == categoryId,
                cancellationToken);
    }

    public async Task AddAsync(
        Medicine medicine,
        CancellationToken cancellationToken)
    {
        await _context.Medicines.AddAsync(
            medicine, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Medicine medicine,
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Medicine medicine,
        CancellationToken cancellationToken)
    {
        _context.Medicines.Remove(medicine);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<Medicine> medicines, int totalCount)> SearchAsync(
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
        CancellationToken cancellationToken)
    {
        // Start with base query: use AsQueryable to allow flexible filtering
        IQueryable<Medicine> query = _context.Medicines
            .AsNoTracking()
            .Include(m => m.Category);

        // Apply filters - all are optional, only apply if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Case-insensitive search by name
            var searchLower = searchTerm.ToLower();
            query = query.Where(m =>
                m.Name.ToLower().Contains(searchLower));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(m => m.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(m => m.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(manufacturer))
        {
            var manufacturerLower = manufacturer.ToLower();
            query = query.Where(m =>
                m.Manufacturer.ToLower().Contains(manufacturerLower));
        }

        if (requiresPrescription.HasValue)
        {
            query = query.Where(m => m.RequiresPrescription == requiresPrescription.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        // Calculate total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortOrder.ToLower() == "desc"
            ? ApplySortingDescending(query, sortBy)
            : ApplySortingAscending(query, sortBy);

        // Apply pagination
        var medicines = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (medicines, totalCount);
    }

    /// <summary>
    /// Apply ascending sorting based on sortBy field.
    /// Supports: name, price, createdAt
    /// </summary>
    private static IQueryable<Medicine> ApplySortingAscending(
        IQueryable<Medicine> query,
        string sortBy)
    {
        return sortBy.ToLower() switch
        {
            "price" => query.OrderBy(m => m.Price),
            "createdat" => query.OrderBy(m => m.CreatedAt),
            _ => query.OrderBy(m => m.Name) // Default to name
        };
    }

    /// <summary>
    /// Apply descending sorting based on sortBy field.
    /// Supports: name, price, createdAt
    /// </summary>
    private static IQueryable<Medicine> ApplySortingDescending(
        IQueryable<Medicine> query,
        string sortBy)
    {
        return sortBy.ToLower() switch
        {
            "price" => query.OrderByDescending(m => m.Price),
            "createdat" => query.OrderByDescending(m => m.CreatedAt),
            _ => query.OrderByDescending(m => m.Name) // Default to name
        };
    }

    public async Task<List<Medicine>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken)
    {
        return await _context.Medicines
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => m.StockQuantity > 0 && m.StockQuantity <= threshold)
            .OrderBy(m => m.StockQuantity)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Medicine>> GetOutOfStockAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Medicines
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => m.StockQuantity == 0)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }
}
