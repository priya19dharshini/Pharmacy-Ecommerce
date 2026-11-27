using Pharmacy_Backend.DTOs.Auth;

namespace Pharmacy_Backend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);

    Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
}