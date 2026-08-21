using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBadgeRepository : IGenericRepository<Badge>
    {
        Task<IEnumerable<UserBadge>> GetUserBadgesAsync(string userId);
        Task<bool> HasBadgeAsync(string userId, int badgeId);
        Task AddUserBadgeAsync(UserBadge userBadge);
    }
}
