using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BloodDriveEventRepository : GenericRepository<BloodDriveEvent>, IBloodDriveEventRepository
    {
        public BloodDriveEventRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<BloodDriveEvent>> GetUpcomingAsync()
        {
            return await _dbSet.AsNoTracking()
                .Include(e => e.BloodBank)
                .Where(e => e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodDriveEvent>> GetByBloodBankAsync(int bloodBankId)
        {
            return await _dbSet.AsNoTracking()
                .Where(e => e.BloodBankId == bloodBankId)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<BloodDriveEvent?> GetWithAttendancesAsync(int eventId)
        {
            return await _dbSet.AsNoTracking()
                .Include(e => e.Attendances)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }
    }
}
