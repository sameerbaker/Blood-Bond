using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class EventAttendanceRepository : GenericRepository<EventAttendance>, IEventAttendanceRepository
    {
        public EventAttendanceRepository(ApplicationDbContext context) : base(context) { }

        public async Task<EventAttendance?> GetAsync(int eventId, string userId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);
        }

        public async Task<IEnumerable<EventAttendance>> GetByEventAsync(int eventId)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventAttendance>> GetByUserAsync(string userId)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.Event)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}
