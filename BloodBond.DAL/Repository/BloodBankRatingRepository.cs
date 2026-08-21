using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BloodBankRatingRepository : GenericRepository<BloodBankRating>, IBloodBankRatingRepository
    {
        public BloodBankRatingRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<BloodBankRating>> GetByBloodBankAsync(int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.BloodBankId == bloodBankId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<BloodBankRating?> GetByUserAndBankAsync(string userId, int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BloodBankId == bloodBankId);
        }

        public async Task<double> GetAverageRatingAsync(int bloodBankId)
        {
            if (!await _dbSet.AnyAsync(r => r.BloodBankId == bloodBankId))
                return 0.0;
            return await _dbSet.AsNoTracking()
                .Where(r => r.BloodBankId == bloodBankId)
                .AverageAsync(r => r.Rating);
        }

        public async Task<int> GetCountAsync(int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .CountAsync(r => r.BloodBankId == bloodBankId);
        }
    }
}
