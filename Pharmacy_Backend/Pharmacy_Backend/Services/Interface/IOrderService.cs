using Pharmacy.API.DTOs.Order;

namespace Pharmacy.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> PlaceOrderAsync(
        string userId, CreateOrderDto dto);

    Task<List<OrderResponseDto>> GetMyOrdersAsync(string userId);

    Task<OrderResponseDto> GetOrderByIdAsync(
        string userId, int orderId, bool isAdmin);

    Task CancelOrderAsync(string userId, int orderId);

    Task<List<OrderResponseDto>> GetAllOrdersAsync();

    Task UpdateOrderStatusAsync(int orderId, string status);
}