
using Microsoft.AspNetCore.Identity;
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Helpers;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var email = configuration["AdminSettings:Email"];
        var password = configuration["AdminSettings:Password"];
        var fullName = configuration["AdminSettings:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "Admin settings are missing from configuration.");
        }

        // Create Admin role if it does not exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole("Admin"));

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Admin role creation failed: {errors}");
            }
        }

        // Check whether Admin account already exists
        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(
                admin, password);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Admin account creation failed: {errors}");
            }
        }

        // Ensure the configured Admin account has the Admin role
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin, "Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Admin role assignment failed: {errors}");
            }
        }
    }
}
