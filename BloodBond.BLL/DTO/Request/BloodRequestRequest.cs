using System;
using System.ComponentModel.DataAnnotations;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Request
{
    public class BloodRequestRequest
    {
        [Required]
        public BloodType BloodType { get; set; }

        [Range(1, 100)]
        public int UnitsNeeded { get; set; }

        [Required]
        public UrgencyLevel UrgencyLevel { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
