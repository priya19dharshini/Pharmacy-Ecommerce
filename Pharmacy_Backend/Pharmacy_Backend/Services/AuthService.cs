
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Pharmacy_Backend.DTOs.Auth;
using Pharmacy_Backend.Helpers;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    // CUSTOMER REGISTRATION
    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
    {
        // 1. Check whether email already exists
        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "Email is already registered.");
        }

        // 2. Create new customer
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address
        };

        var result = await _userManager.CreateAsync(
            user,
            dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        // 3. Create Customer role if it does not exist
        if (!await _roleManager.RoleExistsAsync("Customer"))
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole("Customer"));

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Customer role creation failed: {errors}");
            }
        }

        // 4. Assign Customer role
        var assignResult = await _userManager.AddToRoleAsync(
            user,
            "Customer");

        if (!assignResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                assignResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Customer role assignment failed: {errors}");
        }

        // 5. Generate JWT response
        return await GenerateAuthResponse(user);
    }

    // ADMIN / CUSTOMER LOGIN
    public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
    {
        // 1. Find user by email
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // 2. Check email and password
        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        // 3. Generate JWT response
        return await GenerateAuthResponse(user);
    }

    // GENERATE JWT TOKEN
    private async Task<AuthResponseDTO> GenerateAuthResponse(
        ApplicationUser user)
    {
        // 1. Get assigned roles
        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new InvalidOperationException(
                "User does not have an assigned role.");
        }

        // 2. Create JWT claims
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                ClaimTypes.Name,
                user.FullName ?? string.Empty),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty),

            new Claim(
                ClaimTypes.Role,
                role)
        };

        // 3. Create signing key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // 4. Generate JWT
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryInMinutes),
            signingCredentials: credentials
        );

        // 5. Return authentication response
        return new AuthResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = role
        };
    }
}
