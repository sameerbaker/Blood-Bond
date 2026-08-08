using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BloodInventoryRepository : GenericRepository<BloodInventory>, IBloodInventoryRepository
    {
        public BloodInventoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<BloodInventory?> GetByBankAndTypeAsync(int bloodBankId, BloodType bloodType)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(i => i.BloodBankId == bloodBankId && i.BloodType == bloodType);
        }

        public async Task<IEnumerable<BloodInventory>> GetLowStockAsync(int threshold = 5)
        {
            return await _dbSet.AsNoTracking()
                .Include(i => i.BloodBank)
                .Where(i => i.UnitsAvailable < threshold)
                .ToListAsync();
        }

        public async Task<BloodInventory?> GetWithBankAsync(int inventoryId)
        {
            return await _dbSet.AsNoTracking()
                .Include(i => i.BloodBank)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);
        }
    }
}
