using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Data;

public class PharmacyDbContext : IdentityDbContext<ApplicationUser>
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }
}