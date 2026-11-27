using Microsoft.EntityFrameworkCore;
using Pharmacy.API.Data;
using Pharmacy.API.DTOs.Cart;
using Pharmacy.API.Models;
using Pharmacy.API.Repositories.Interfaces;
using Pharmacy.API.Services.Interfaces;

namespace Pharmacy.API.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly PharmacyDbContext _context;

    public CartService(
        ICartRepository cartRepository,
        PharmacyDbContext context)
    {
        _cartRepository = cartRepository;
        _context = context;
    }

    public async Task<CartResponseDto> GetCartAsync(string userId)
    {
        var cart = await _cartRepository.GetOrCreateCartAsync(userId);

        return MapCart(cart);
    }

    public async Task<CartResponseDto> AddToCartAsync(
        string userId, AddToCartDto dto)
    {
        var medicine = await _context.Medicines
            .FirstOrDefaultAsync(m => m.Id == dto.MedicineId);

        if (medicine == null)
            throw new KeyNotFoundException("Medicine not found.");

        if (!medicine.IsActive)
            throw new InvalidOperationException("Medicine is unavailable.");

        if (medicine.StockQuantity < dto.Quantity)
            throw new InvalidOperationException("Insufficient stock.");

        var cart = await _cartRepository.GetOrCreateCartAsync(userId);

        var existingItem = cart.CartItems
            .FirstOrDefault(x => x.MedicineId == dto.MedicineId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + dto.Quantity;

            if (newQuantity > medicine.StockQuantity)
                throw new InvalidOperationException("Insufficient stock.");

            existingItem.Quantity = newQuantity;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                MedicineId = medicine.Id,
                Quantity = dto.Quantity
            });
        }

        await _cartRepository.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    public async Task<CartResponseDto> UpdateCartItemAsync(
        string userId, int itemId, UpdateCartItemDto dto)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        var item = cart?.CartItems.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
            throw new KeyNotFoundException("Cart item not found.");

        if (item.Medicine.StockQuantity < dto.Quantity)
            throw new InvalidOperationException("Insufficient stock.");

        item.Quantity = dto.Quantity;

        await _cartRepository.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    public async Task RemoveCartItemAsync(string userId, int itemId)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        var item = cart?.CartItems.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
            throw new KeyNotFoundException("Cart item not found.");

        _context.CartItems.Remove(item);

        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await _cartRepository.GetCartAsync(userId);

        if (cart == null)
            return;

        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();
    }

    private static CartResponseDto MapCart(Cart cart)
    {
        return new CartResponseDto
        {
            CartId = cart.Id,

            Items = cart.CartItems.Select(item =>
                new CartItemResponseDto
                {
                    CartItemId = item.Id,
                    MedicineId = item.MedicineId,
                    MedicineName = item.Medicine.Name,
                    UnitPrice = item.Medicine.Price,
                    Quantity = item.Quantity,
                    SubTotal = item.Medicine.Price * item.Quantity
                }).ToList(),

            TotalAmount = cart.CartItems.Sum(
                item => item.Medicine.Price * item.Quantity)
        };
    }
}
