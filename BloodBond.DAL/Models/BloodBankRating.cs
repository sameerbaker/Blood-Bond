using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBond.DAL.Models
{
    public class BloodBankRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BloodBankId { get; set; }
        [ForeignKey(nameof(BloodBankId))]
        public BloodBank? BloodBank { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
