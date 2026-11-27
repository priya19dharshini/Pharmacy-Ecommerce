using Pharmacy_Backend.DTOs.Users;

namespace Pharmacy_Backend.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileDTO?> GetProfileAsync(string userId);

    Task<UserProfileDTO?> UpdateProfileAsync(
        string userId,
        UpdateUserDTO dto);
}