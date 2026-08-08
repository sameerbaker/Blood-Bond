using System;
using System.ComponentModel.DataAnnotations;
using BloodBond.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace BloodBond.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(80)]
        public string FullName { get; set; } = string.Empty;

        public BloodType? BloodType { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public int Points { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsBlocked { get; set; } = false;
    }
}
