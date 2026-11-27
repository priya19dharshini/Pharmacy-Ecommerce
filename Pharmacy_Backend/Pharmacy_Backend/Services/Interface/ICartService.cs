using Pharmacy.API.DTOs.Cart;

namespace Pharmacy.API.Services.Interfaces;

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(string userId);

    Task<CartResponseDto> AddToCartAsync(
        string userId, AddToCartDto dto);

    Task<CartResponseDto> UpdateCartItemAsync(
        string userId, int itemId, UpdateCartItemDto dto);

    Task RemoveCartItemAsync(string userId, int itemId);

    Task ClearCartAsync(string userId);
}