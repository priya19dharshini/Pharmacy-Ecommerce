using Microsoft.AspNetCore.Identity;
using Pharmacy_Backend.DTOs.Users;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileDTO?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        return new UserProfileDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };
    }

    public async Task<UserProfileDTO?> UpdateProfileAsync(
        string userId,
        UpdateUserDTO dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return await GetProfileAsync(userId);
    }
}