using Microsoft.EntityFrameworkCore;
using Pharmacy.API.Data;
using Pharmacy.API.Models;
using Pharmacy.API.Repositories.Interfaces;

namespace Pharmacy.API.Repositories.Implementations;

public class CartRepository : ICartRepository
{
    private readonly PharmacyDbContext _context;

    public CartRepository(PharmacyDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartAsync(string userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Medicine)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await GetCartAsync(userId);

        if (cart != null)
            return cart;

        cart = new Cart { UserId = userId };

        _context.Carts.Add(cart);

        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}