
using Pharmacy_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Data
{
    public class PharmacyDbContext : DbContext
    {
        public PharmacyDbContext(
            DbContextOptions<PharmacyDbContext> options)
            : base(options)
        {
        }

        // User
        public DbSet<ApplicationUser> Users { get; set; }

        // Medicine
        public DbSet<Medicine> Medicines { get; set; }

        // Category
        public DbSet<Category> Categories { get; set; }

        // Cart
        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        // Orders
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        // Prescription - Member 4
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prescription configuration
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.UserId)
                    .IsRequired();

                entity.Property(p => p.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(p => p.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(p => p.RejectionReason)
                    .HasMaxLength(500);
            });
        }
    }
}
