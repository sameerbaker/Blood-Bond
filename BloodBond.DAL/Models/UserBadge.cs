using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBond.DAL.Models
{
    public class UserBadge
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        public int BadgeId { get; set; }
        [ForeignKey(nameof(BadgeId))]
        public Badge? Badge { get; set; }

        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
