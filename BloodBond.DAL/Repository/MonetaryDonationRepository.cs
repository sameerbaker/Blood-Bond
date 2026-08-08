using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class MonetaryDonationRepository : GenericRepository<MonetaryDonation>, IMonetaryDonationRepository
    {
        public MonetaryDonationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<MonetaryDonation>> GetByDonorAsync(string donorId)
        {
            return await _dbSet.AsNoTracking()
                .Include(m => m.BloodBank)
                .Where(m => m.DonorId == donorId)
                .OrderByDescending(m => m.DonationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonetaryDonation>> GetByBloodBankAsync(int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .Include(m => m.Donor)
                .Where(m => m.BloodBankId == bloodBankId && m.Status == "Succeeded")
                .OrderByDescending(m => m.DonationDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalByDonorAsync(string donorId)
        {
            return await _dbSet.AsNoTracking()
                .Where(m => m.DonorId == donorId && m.Status == "Succeeded")
                .SumAsync(m => m.Amount);
        }
    }
}
