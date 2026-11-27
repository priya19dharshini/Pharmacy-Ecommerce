using Microsoft.EntityFrameworkCore;
using Pharmacy.API.Data;
using Pharmacy.API.DTOs.Order;
using Pharmacy.API.Models;
using Pharmacy.API.Repositories.Interfaces;
using Pharmacy.API.Services.Interfaces;

namespace Pharmacy.API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly PharmacyDbContext _context;
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        PharmacyDbContext context,
        ICartRepository cartRepository,
        IOrderRepository orderRepository)
    {
        _context = context;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> PlaceOrderAsync(
        string userId, CreateOrderDto dto)
    {
        if (dto.PaymentMethod != "COD")
            throw new InvalidOperationException(
                "Only COD payment is supported currently.");

        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart == null || !cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            decimal total = 0;

            foreach (var item in cart.CartItems)
            {
                var medicine = await _context.Medicines
                    .FirstOrDefaultAsync(m => m.Id == item.MedicineId);

                if (medicine == null || !medicine.IsActive)
                    throw new InvalidOperationException(
                        "One or more medicines are unavailable.");

                if (medicine.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for {medicine.Name}.");

                total += medicine.Price * item.Quantity;
            }

            // Prescription validation integration:
            // Coordinate with Member 4 here.
            // Do not confirm a prescription-required order
            // unless the prescription is approved.

            var order = new Order
            {
                UserId = userId,
                DeliveryAddress = dto.DeliveryAddress,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = total,
                Status = OrderStatus.Placed,
                OrderDate = DateTime.UtcNow
            };

            foreach (var item in cart.CartItems)
            {
                var medicine = await _context.Medicines
                    .FirstAsync(m => m.Id == item.MedicineId);

                order.OrderItems.Add(new OrderItem
                {
                    MedicineId = medicine.Id,
                    Quantity = item.Quantity,
                    UnitPrice = medicine.Price
                });

                // Simple stock deduction.
                medicine.StockQuantity -= item.Quantity;
            }

            await _orderRepository.AddOrderAsync(order);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _orderRepository.SaveChangesAsync();

            await transaction.CommitAsync();

            var savedOrder = await _orderRepository
                .GetOrderByIdAsync(order.Id);

            return MapOrder(savedOrder!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderResponseDto>> GetMyOrdersAsync(
        string userId)
    {
        var orders = await _orderRepository.GetOrdersByUserAsync(userId);

        return orders.Select(MapOrder).ToList();
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(
        string userId, int orderId, bool isAdmin)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (!isAdmin && order.UserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        return MapOrder(order);
    }

    public async Task CancelOrderAsync(string userId, int orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null || order.UserId != userId)
            throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.Placed)
            throw new InvalidOperationException(
                "Only placed orders can be cancelled.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in order.OrderItems)
            {
                var medicine = await _context.Medicines
                    .FirstAsync(m => m.Id == item.MedicineId);

                medicine.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();

        return orders.Select(MapOrder).ToList();
    }

    public async Task UpdateOrderStatusAsync(
        int orderId, string status)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (!Enum.TryParse<OrderStatus>(
            status, true, out var newStatus))
            throw new ArgumentException("Invalid order status.");

        // Prevent invalid transitions.
        if (order.Status == OrderStatus.Delivered ||
            order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException(
                "This order cannot be updated.");

        order.Status = newStatus;

        await _orderRepository.SaveChangesAsync();
    }

    private static OrderResponseDto MapOrder(Order order)
    {
        return new OrderResponseDto
        {
            OrderId = order.Id,
            DeliveryAddress = order.DeliveryAddress,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,

            Items = order.OrderItems.Select(item =>
                new OrderItemResponseDto
                {
                    MedicineId = item.MedicineId,
                    MedicineName = item.Medicine.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    SubTotal = item.UnitPrice * item.Quantity
                }).ToList()
        };
    }
}