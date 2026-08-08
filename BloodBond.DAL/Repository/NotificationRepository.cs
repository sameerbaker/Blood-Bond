using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserAsync(string userId)
        {
            return await _dbSet.AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _dbSet.AsNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}
