using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pharmacy_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Pharmacy_Backend.Data
{
    public class PharmacyDbContext : IdentityDbContext<ApplicationUser>
    {
        public PharmacyDbContext(
            DbContextOptions<PharmacyDbContext> options)
            : base(options)
        {
        }

        // Medicine
        public DbSet<Medicine> Medicines { get; set; }

        // Category
        public DbSet<Category> Categories { get; set; }

        // Prescription
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
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
