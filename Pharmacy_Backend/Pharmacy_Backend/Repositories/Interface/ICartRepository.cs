using Pharmacy.API.Models;

namespace Pharmacy.API.Repositories.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartAsync(string userId);

    Task<Cart> GetOrCreateCartAsync(string userId);

    Task SaveChangesAsync();
}