using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
﻿
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Data;

public class PharmacyDbContext : DbContext
{
    public PharmacyDbContext(
        DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Medicine> Medicines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Medicine>()
            .HasOne(m => m.Category)
            .WithMany()
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Medicine>()
            .Property(m => m.Price)
            .HasPrecision(18, 2);
    }
}
