using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.Models
{
    public class EventAttendance
    {
        [Required]
        public int EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public BloodDriveEvent? Event { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public CheckInStatus Status { get; set; } = CheckInStatus.Registered;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? CheckedInAt { get; set; }
    }
}
