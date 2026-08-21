using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.DAL.Repository
{
    public class BadgeRepository : GenericRepository<Badge>, IBadgeRepository
    {
        private readonly ApplicationDbContext _ctx;
        public BadgeRepository(ApplicationDbContext context) : base(context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(string userId)
        {
            return await _ctx.UserBadges
                .AsNoTracking()
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserId == userId)
                .OrderByDescending(ub => ub.EarnedAt)
                .ToListAsync();
        }

        public async Task<bool> HasBadgeAsync(string userId, int badgeId)
        {
            return await _ctx.UserBadges
                .AsNoTracking()
                .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
        }

        public async Task AddUserBadgeAsync(UserBadge userBadge)
        {
            await _ctx.UserBadges.AddAsync(userBadge);
        }
    }
}
