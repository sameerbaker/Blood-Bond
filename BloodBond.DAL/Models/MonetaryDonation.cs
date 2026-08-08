using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBond.DAL.Models
{
    public class MonetaryDonation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DonorId { get; set; } = string.Empty;
        [ForeignKey(nameof(DonorId))]
        public ApplicationUser? Donor { get; set; }

        public int? BloodBankId { get; set; }
        [ForeignKey(nameof(BloodBankId))]
        public BloodBank? BloodBank { get; set; }

        [Required, Range(0.01, 1_000_000)]
        public decimal Amount { get; set; }

        [Required, MaxLength(10)]
        public string Currency { get; set; } = "USD";

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? StripePaymentIntentId { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; 
    }
}
