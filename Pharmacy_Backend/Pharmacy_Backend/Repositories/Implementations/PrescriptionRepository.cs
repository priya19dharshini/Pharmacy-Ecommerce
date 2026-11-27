
using Microsoft.EntityFrameworkCore;
using Pharmacy_Backend.Data;
using Pharmacy_Backend.Models;
using Pharmacy_Backend.Repositories.Interfaces;

namespace Pharmacy_Backend.Repositories.Implementations
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly PharmacyDbContext _context;

        public PrescriptionRepository(PharmacyDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription> AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();

            return prescription;
        }

        public async Task<Prescription?> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Prescription>> GetByUserIdAsync(string userId)
        {
            return await _context.Prescriptions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();
        }

        public async Task<List<Prescription>> GetAllAsync()
        {
            return await _context.Prescriptions
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();
        }
    }
}
