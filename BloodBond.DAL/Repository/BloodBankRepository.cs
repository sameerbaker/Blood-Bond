using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BloodBankRepository : GenericRepository<BloodBank>, IBloodBankRepository
    {
        public BloodBankRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<BloodBank>> GetByStatusAsync(BloodBankStatus status)
        {
            return await _dbSet.AsNoTracking()
                .Include(b => b.Inventory)
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        public async Task<BloodBank?> GetByManagerIdAsync(string managerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(b => b.Inventory)
                .FirstOrDefaultAsync(b => b.ManagerId == managerId);
        }

        public async Task<IEnumerable<BloodBank>> GetVerifiedByCityAsync(string city)
        {
            return await _dbSet.AsNoTracking()
                .Include(b => b.Inventory)
                .Where(b => b.Status == BloodBankStatus.Verified && b.CityAddress.Contains(city))
                .ToListAsync();
        }
    }
}
