
using Pharmacy_Backend.DTOs.Medicine;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _repository;

    public MedicineService(IMedicineRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MedicineResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var medicines = await _repository.GetAllAsync(
            cancellationToken);

        return medicines.Select(MapToResponse).ToList();
    }

    public async Task<MedicineResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var medicine = await _repository.GetByIdAsync(
            id, cancellationToken);

        return medicine == null
            ? null
            : MapToResponse(medicine);
    }

    public async Task<MedicineResponseDto> CreateAsync(
        CreateMedicineDto dto,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _repository.CategoryExistsAsync(
            dto.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "Selected category does not exist.");
        }

        var medicine = new Medicine
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl?.Trim(),
            RequiresPrescription = dto.RequiresPrescription,
            CategoryId = dto.CategoryId,
            Manufacturer = dto.Manufacturer.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
            medicine, cancellationToken);

        var savedMedicine = await _repository.GetByIdAsync(
            medicine.Id, cancellationToken);

        return MapToResponse(savedMedicine!);
    }

    public async Task<MedicineResponseDto?> UpdateAsync(
        int id,
        UpdateMedicineDto dto,
        CancellationToken cancellationToken)
    {
        var medicine = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (medicine == null)
        {
            return null;
        }

        var categoryExists = await _repository.CategoryExistsAsync(
            dto.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "Selected category does not exist.");
        }

        medicine.Name = dto.Name.Trim();
        medicine.Description = dto.Description?.Trim();
        medicine.Price = dto.Price;
        medicine.StockQuantity = dto.StockQuantity;
        medicine.ImageUrl = dto.ImageUrl?.Trim();
        medicine.RequiresPrescription = dto.RequiresPrescription;
        medicine.CategoryId = dto.CategoryId;
        medicine.Manufacturer = dto.Manufacturer.Trim();
        medicine.IsActive = dto.IsActive;

        await _repository.UpdateAsync(
            medicine, cancellationToken);

        return MapToResponse(medicine);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var medicine = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (medicine == null)
        {
            return false;
        }

        await _repository.DeleteAsync(
            medicine, cancellationToken);

        return true;
    }

    public async Task<PagedMedicineResponseDto> SearchAsync(
        MedicineSearchDto dto,
        CancellationToken cancellationToken)
    {
        // Validate pagination
        if (dto.PageNumber < 1)
        {
            throw new ArgumentException(
                "Page number must be at least 1.", nameof(dto.PageNumber));
        }

        if (dto.PageSize < 1 || dto.PageSize > 100)
        {
            throw new ArgumentException(
                "Page size must be between 1 and 100.", nameof(dto.PageSize));
        }

        // Validate price range
        if (dto.MinPrice.HasValue && dto.MinPrice.Value < 0)
        {
            throw new ArgumentException(
                "Minimum price cannot be negative.", nameof(dto.MinPrice));
        }

        if (dto.MaxPrice.HasValue && dto.MaxPrice.Value < 0)
        {
            throw new ArgumentException(
                "Maximum price cannot be negative.", nameof(dto.MaxPrice));
        }

        if (dto.MinPrice.HasValue && dto.MaxPrice.HasValue &&
            dto.MinPrice.Value > dto.MaxPrice.Value)
        {
            throw new ArgumentException(
                "Minimum price cannot exceed maximum price.");
        }

        // Validate sort field
        var validSortFields = new[] { "name", "price", "createdAt" };
        if (!validSortFields.Contains(dto.SortBy.ToLower()))
        {
            throw new ArgumentException(
                $"Invalid sort field. Allowed values: {string.Join(", ", validSortFields)}", 
                nameof(dto.SortBy));
        }

        // Validate sort order
        var validSortOrders = new[] { "asc", "desc" };
        if (!validSortOrders.Contains(dto.SortOrder.ToLower()))
        {
            throw new ArgumentException(
                $"Invalid sort order. Allowed values: {string.Join(", ", validSortOrders)}", 
                nameof(dto.SortOrder));
        }

        // Call repository search with all parameters
        var (medicines, totalCount) = await _repository.SearchAsync(
            dto.SearchTerm,
            dto.CategoryId,
            dto.MinPrice,
            dto.MaxPrice,
            dto.Manufacturer,
            dto.RequiresPrescription,
            dto.IsActive,
            dto.SortBy,
            dto.SortOrder,
            dto.PageNumber,
            dto.PageSize,
            cancellationToken);

        // Map results to DTOs
        var medicinesDtos = medicines.Select(MapToResponse).ToList();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

        return new PagedMedicineResponseDto
        {
            Data = medicinesDtos,
            TotalCount = totalCount,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalPages = totalPages
        };
    }

    private static MedicineResponseDto MapToResponse(
        Medicine medicine)
    {
        return new MedicineResponseDto
        {
            Id = medicine.Id,
            Name = medicine.Name,
            Description = medicine.Description,
            Price = medicine.Price,
            StockQuantity = medicine.StockQuantity,
            ImageUrl = medicine.ImageUrl,
            RequiresPrescription = medicine.RequiresPrescription,
            CategoryId = medicine.CategoryId,
            CategoryName = medicine.Category.Name,
            Manufacturer = medicine.Manufacturer,
            IsActive = medicine.IsActive,
            CreatedAt = medicine.CreatedAt
        };
    }

    public async Task<MedicineStockResponseDto?> UpdateStockAsync(
        int medicineId,
        UpdateStockDto dto,
        CancellationToken cancellationToken)
    {
        // Validate operation
        var validOperations = new[] { "increase", "decrease", "set" };
        if (!validOperations.Contains(dto.Operation.ToLower()))
        {
            throw new ArgumentException(
                $"Invalid operation. Allowed values: Increase, Decrease, Set",
                nameof(dto.Operation));
        }

        // Validate quantity
        if (dto.Quantity < 0)
        {
            throw new ArgumentException(
                "Quantity cannot be negative.",
                nameof(dto.Quantity));
        }

        // For Increase and Decrease, quantity must be > 0
        if ((dto.Operation.ToLower() == "increase" || dto.Operation.ToLower() == "decrease") &&
            dto.Quantity == 0)
        {
            throw new ArgumentException(
                $"{dto.Operation} quantity must be greater than zero.",
                nameof(dto.Quantity));
        }

        // Get the medicine
        var medicine = await _repository.GetByIdAsync(
            medicineId, cancellationToken);

        if (medicine == null)
        {
            return null;
        }

        // Apply the operation
        var newStock = dto.Operation.ToLower() switch
        {
            "increase" => medicine.StockQuantity + dto.Quantity,
            "decrease" => medicine.StockQuantity - dto.Quantity,
            "set" => dto.Quantity,
            _ => medicine.StockQuantity // Should not reach here due to validation
        };

        // Validate the result
        if (newStock < 0)
        {
            throw new ArgumentException(
                $"Operation would result in negative stock. Current stock: {medicine.StockQuantity}, " +
                $"operation: {dto.Operation}, quantity: {dto.Quantity}");
        }

        // Check for integer overflow on increase
        if (dto.Operation.ToLower() == "increase" && newStock > int.MaxValue)
        {
            throw new ArgumentException(
                $"Operation would result in stock quantity exceeding maximum value.");
        }

        // Update the stock
        medicine.StockQuantity = newStock;

        // Save changes through repository
        await _repository.UpdateAsync(medicine, cancellationToken);

        // Return the updated stock information
        return MapToStockResponse(medicine);
    }

    public async Task<List<MedicineStockResponseDto>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken)
    {
        // Validate threshold
        if (threshold < 0)
        {
            throw new ArgumentException(
                "Threshold cannot be negative.",
                nameof(threshold));
        }

        var medicines = await _repository.GetLowStockAsync(
            threshold, cancellationToken);

        return medicines.Select(MapToStockResponse).ToList();
    }

    public async Task<List<MedicineStockResponseDto>> GetOutOfStockAsync(
        CancellationToken cancellationToken)
    {
        var medicines = await _repository.GetOutOfStockAsync(
            cancellationToken);

        return medicines.Select(MapToStockResponse).ToList();
    }

    /// <summary>
    /// Map Medicine entity to MedicineStockResponseDto with calculated stock status.
    /// Stock status rules:
    /// - OutOfStock: StockQuantity == 0
    /// - LowStock: StockQuantity between 1 and 10 (inclusive)
    /// - InStock: StockQuantity > 10
    /// </summary>
    private static MedicineStockResponseDto MapToStockResponse(
        Medicine medicine)
    {
        var stockStatus = medicine.StockQuantity switch
        {
            0 => "OutOfStock",
            >= 1 and <= 10 => "LowStock",
            _ => "InStock"
        };

        return new MedicineStockResponseDto
        {
            MedicineId = medicine.Id,
            MedicineName = medicine.Name,
            StockQuantity = medicine.StockQuantity,
            StockStatus = stockStatus
        };
    }
}
