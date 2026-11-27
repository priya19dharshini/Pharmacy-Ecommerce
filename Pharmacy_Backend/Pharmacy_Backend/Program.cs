<<<<<<< Updated upstream

using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Data;
using Pharmacy_Backend.Repositories;
using Pharmacy_Backend.Repositories.Interfaces;

using Pharmacy_Backend.Services;
using Pharmacy_Backend.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

=======
using Pharmacy.API.Repositories.Interfaces;
using Pharmacy.API.Repositories.Implementations;
using Pharmacy.API.Services.Interfaces;
using Pharmacy.API.Services.Implementations;

// Repository registration
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Service registration
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();


>>>>>>> Stashed changes
builder.Services.AddControllers();

builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();

builder.Services.AddScoped<IMedicineService, MedicineService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
