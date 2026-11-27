namespace Pharmacy_Backend.DTOs.Users;

public class UserDTO
{
    public string Id { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}