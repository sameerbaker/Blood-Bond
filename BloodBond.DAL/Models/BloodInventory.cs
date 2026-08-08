using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.Models
{
    public class BloodInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BloodBankId { get; set; }
        [ForeignKey(nameof(BloodBankId))]
        public BloodBank? BloodBank { get; set; }

        [Required]
        public BloodType BloodType { get; set; }

        [Range(0, int.MaxValue)]
        public int UnitsAvailable { get; set; }

        public DateTime LastUpdated { get; set; } = System.DateTime.UtcNow;
    }
}
