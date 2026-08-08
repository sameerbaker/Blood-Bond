using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DonorId { get; set; } = string.Empty;
        [ForeignKey(nameof(DonorId))]
        public ApplicationUser? Donor { get; set; }

        [Required]
        public int BloodBankId { get; set; }
        [ForeignKey(nameof(BloodBankId))]
        public BloodBank? BloodBank { get; set; }

        public int? RequestId { get; set; }
        [ForeignKey(nameof(RequestId))]
        public BloodRequest? Request { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? DonationDate { get; set; }

        [Range(0, 10)]
        public int UnitsDonated { get; set; }

        [Required]
        public DonationStatus Status { get; set; } = DonationStatus.Scheduled;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
