using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IBadgeService
    {
        Task<IEnumerable<BadgeResponse>> GetAllBadgesAsync();
        Task<IEnumerable<UserBadgeResponse>> GetUserBadgesAsync(string userId);
        Task<LeaderboardEntryResponse?> GetMyRankAsync(string userId);
        Task<IEnumerable<LeaderboardEntryResponse>> GetLeaderboardAsync(int top = 10);
        Task CheckAndAwardBadgesAsync(string userId);
    }
}
