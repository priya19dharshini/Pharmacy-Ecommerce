
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Pharmacy_Backend.DTOs.Auth;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // CUSTOMER REGISTRATION
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);

        return Ok(result);
    }

    // CUSTOMER / ADMIN LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);

        return Ok(result);
    }
}
