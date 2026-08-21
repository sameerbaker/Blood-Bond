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
        /// <summary>Check the user's points and award any new badges they qualify for.</summary>
        Task CheckAndAwardBadgesAsync(string userId);
    }
}
