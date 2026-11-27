using System.ComponentModel.DataAnnotations;

namespace Pharmacy_Backend.DTOs.Users;

public class UpdateUserDTO
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}