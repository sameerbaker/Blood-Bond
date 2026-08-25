using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using BloodBond.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.BLL.Service
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepo;
        private readonly ApplicationDbContext _context;

        public BadgeService(IBadgeRepository badgeRepo, ApplicationDbContext context)
        {
            _badgeRepo = badgeRepo;
            _context = context;
        }

        public async Task<IEnumerable<BadgeResponse>> GetAllBadgesAsync()
        {
            var badges = await _context.Badges.AsNoTracking().ToListAsync();
            return badges.Select(b => new BadgeResponse
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Icon = b.Icon,
                PointsRequired = b.PointsRequired
            });
        }

        public async Task<IEnumerable<UserBadgeResponse>> GetUserBadgesAsync(string userId)
        {
            var userBadges = await _badgeRepo.GetUserBadgesAsync(userId);
            return userBadges.Select(ub => new UserBadgeResponse
            {
                BadgeId = ub.BadgeId,
                Name = ub.Badge?.Name ?? "",
                Description = ub.Badge?.Description,
                Icon = ub.Badge?.Icon ?? "🏅",
                EarnedAt = ub.EarnedAt
            });
        }

        public async Task<LeaderboardEntryResponse?> GetMyRankAsync(string userId)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var donations = await _context.Donations
                .CountAsync(d => d.DonorId == userId && d.Status == DonationStatus.Completed);
            var badgeCount = await _context.UserBadges
                .CountAsync(ub => ub.UserId == userId);

            return new LeaderboardEntryResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Points = user.Points,
                TotalDonations = donations,
                BadgeCount = badgeCount
            };
        }

        public async Task<IEnumerable<LeaderboardEntryResponse>> GetLeaderboardAsync(int top = 10)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.Points > 0)
                .OrderByDescending(u => u.Points)
                .Take(top)
                .ToListAsync();

            var result = new List<LeaderboardEntryResponse>();
            foreach (var u in users)
            {
                var donations = await _context.Donations
                    .CountAsync(d => d.DonorId == u.Id && d.Status == DonationStatus.Completed);
                var badgeCount = await _context.UserBadges
                    .CountAsync(ub => ub.UserId == u.Id);
                result.Add(new LeaderboardEntryResponse
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Points = u.Points,
                    TotalDonations = donations,
                    BadgeCount = badgeCount
                });
            }
            return result;
        }

        public async Task CheckAndAwardBadgesAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;

            var allBadges = await _context.Badges.AsNoTracking().ToListAsync();
            var earnedIds = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BadgeId)
                .ToListAsync();

            foreach (var badge in allBadges)
            {
                if (earnedIds.Contains(badge.Id))
                    continue; 

                bool qualifies = false;
                if (badge.Name == "Patron")
                {
                    qualifies = await _context.MonetaryDonations
                        .AnyAsync(m => m.DonorId == userId && m.Status == "Succeeded");
                }
                else
                {
                    qualifies = user.Points >= badge.PointsRequired;
                }

                if (qualifies)
                {
                    await _badgeRepo.AddUserBadgeAsync(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
