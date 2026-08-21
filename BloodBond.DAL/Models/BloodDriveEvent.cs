using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBond.DAL.Models
{
    public class BloodDriveEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BloodBankId { get; set; }
        [ForeignKey(nameof(BloodBankId))]
        public BloodBank? BloodBank { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int Capacity { get; set; } = 50;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();
    }
}
