
using Pharmacy_Backend.Models;

namespace Pharmacy_Backend.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> AddAsync(Prescription prescription);

        Task<Prescription?> GetByIdAsync(int id);

        Task<List<Prescription>> GetByUserIdAsync(string userId);

        Task<List<Prescription>> GetAllAsync();

        Task UpdateAsync(Prescription prescription);
    }
}
