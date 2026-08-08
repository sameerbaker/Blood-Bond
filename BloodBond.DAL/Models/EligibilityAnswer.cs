using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBond.DAL.Models
{
    public class EligibilityAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Range(0, 500)]
        public double Weight { get; set; }

        [Range(16, 100)]
        public int Age { get; set; }

        public bool HasChronicDisease { get; set; }

        public DateTime? LastSurgeryDate { get; set; }

        public bool Passed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
