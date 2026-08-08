using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Donation>> GetByDonorAsync(string donorId)
        {
            return await _dbSet.AsNoTracking()
                .Include(d => d.BloodBank)
                .Where(d => d.DonorId == donorId)
                .OrderByDescending(d => d.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetByBloodBankAsync(int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .Include(d => d.Donor)
                .Where(d => d.BloodBankId == bloodBankId)
                .OrderByDescending(d => d.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetByStatusAsync(DonationStatus status)
        {
            return await _dbSet.AsNoTracking()
                .Where(d => d.Status == status)
                .ToListAsync();
        }
    }
}
