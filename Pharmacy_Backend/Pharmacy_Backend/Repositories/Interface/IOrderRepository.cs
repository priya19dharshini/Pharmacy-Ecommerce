using Pharmacy.API.Models;

namespace Pharmacy.API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);

    Task<Order?> GetOrderByIdAsync(int id);

    Task<List<Order>> GetOrdersByUserAsync(string userId);

    Task<List<Order>> GetAllOrdersAsync();

    Task SaveChangesAsync();
}