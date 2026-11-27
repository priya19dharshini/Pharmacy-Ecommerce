
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Pharmacy_Backend.Data;
using Pharmacy_Backend.Helpers;
using Pharmacy_Backend.Repositories.Interfaces;
using Pharmacy_Backend.Repositories.Implementations;
using Pharmacy_Backend.Services.Interfaces;
using Pharmacy_Backend.Services.Implementations;
using Pharmacy_Backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// 1. CONTROLLERS
// ------------------------------------

builder.Services.AddControllers();

// ------------------------------------
// 2. DATABASE CONNECTION
// ------------------------------------

builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ------------------------------------
// 3. CORS CONFIGURATION
// ------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ------------------------------------
// 4. JWT AUTHENTICATION
// ------------------------------------

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT key must be configured with at least 32 bytes."
    );
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                ),

                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

// ------------------------------------
// 5. AUTHORIZATION
// ------------------------------------

builder.Services.AddAuthorization();

// ------------------------------------
// 6. PRESCRIPTION DEPENDENCY INJECTION
// ------------------------------------

builder.Services.AddScoped<
    IPrescriptionRepository,
    PrescriptionRepository>();

builder.Services.AddScoped<
    IPrescriptionService,
    PrescriptionService>();

builder.Services.AddScoped<FileUploadHelper>();

// ------------------------------------
// 7. OTHER MODULE DEPENDENCIES
// ------------------------------------

// Register your existing Auth, User, Medicine,
// Category, Cart and Order services here.
// Keep the registrations already created by your team.

// ------------------------------------
// 8. SWAGGER
// ------------------------------------

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------------
// 9. MIDDLEWARE
// ------------------------------------

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ------------------------------------
// 10. CORS
// ------------------------------------

app.UseCors("AllowAngular");

// ------------------------------------
// 11. AUTHENTICATION
// ------------------------------------

app.UseAuthentication();

// ------------------------------------
// 12. AUTHORIZATION
// ------------------------------------

app.UseAuthorization();

// ------------------------------------
// 13. MAP CONTROLLERS
// ------------------------------------

app.MapControllers();

app.Run();