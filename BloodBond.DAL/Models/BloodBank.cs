using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.Models
{
    public class BloodBank : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string CityAddress { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        public BloodBankStatus Status { get; set; } = BloodBankStatus.Pending;

        // Manager of this blood bank (a registered user with the right role)
        [Required]
        public string ManagerId { get; set; } = string.Empty;
        [ForeignKey(nameof(ManagerId))]
        public ApplicationUser? Manager { get; set; }

        public ICollection<BloodInventory> Inventory { get; set; } = new List<BloodInventory>();

       
        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser? CreatedBy { get; set; }

        [ForeignKey(nameof(UpdatedById))]
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
