using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.Models
{
    public class BloodRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RequesterId { get; set; } = string.Empty;
        [ForeignKey(nameof(RequesterId))]
        public ApplicationUser? Requester { get; set; }

        [Required]
        public BloodType BloodType { get; set; }

        [Range(1, 100)]
        public int UnitsNeeded { get; set; }

        [Required]
        public UrgencyLevel UrgencyLevel { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
