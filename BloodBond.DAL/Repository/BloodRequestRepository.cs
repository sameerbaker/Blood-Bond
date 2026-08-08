using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BloodRequestRepository : GenericRepository<BloodRequest>, IBloodRequestRepository
    {
        public BloodRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<BloodRequest>> GetByRequesterAsync(string requesterId)
        {
            return await _dbSet.AsNoTracking()
                .Where(r => r.RequesterId == requesterId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetActiveByCityAsync(string city)
        {
            return await _dbSet.AsNoTracking()
                .Where(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress)
                .Where(r => r.City == city)
                .OrderByDescending(r => r.UrgencyLevel)
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetByUrgencyAsync(UrgencyLevel level)
        {
            return await _dbSet.AsNoTracking()
                .Where(r => r.UrgencyLevel == level && r.Status == RequestStatus.Pending)
                .ToListAsync();
        }
    }
}
