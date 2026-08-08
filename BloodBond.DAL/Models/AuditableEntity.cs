using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.Models
{
    
    public abstract class AuditableEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
