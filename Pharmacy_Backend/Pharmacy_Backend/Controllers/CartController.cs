using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Pharmacy.API.DTOs.Cart;
using Pharmacy.API.Services.Interfaces;

namespace Pharmacy.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCartAsync(UserId);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart(
        AddToCartDto dto)
    {
        var result = await _cartService.AddToCartAsync(UserId, dto);
        return Ok(result);
    }

    [HttpPut("items/{itemId:int}")]
    public async Task<IActionResult> UpdateCartItem(
        int itemId, UpdateCartItemDto dto)
    {
        var result = await _cartService.UpdateCartItemAsync(
            UserId, itemId, dto);

        return Ok(result);
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemoveCartItem(int itemId)
    {
        await _cartService.RemoveCartItemAsync(UserId, itemId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(UserId);
        return NoContent();
    }
}