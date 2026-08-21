using System;

namespace BloodBond.DAL.DTO.Response
{
    public class BadgeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Icon { get; set; } = "🏅";
        public int PointsRequired { get; set; }
    }

    public class UserBadgeResponse
    {
        public int BadgeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Icon { get; set; } = "🏅";
        public DateTime EarnedAt { get; set; }
    }

    public class LeaderboardEntryResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Points { get; set; }
        public int TotalDonations { get; set; }
        public int BadgeCount { get; set; }
    }
}
