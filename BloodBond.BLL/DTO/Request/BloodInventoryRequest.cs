using System.ComponentModel.DataAnnotations;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Request
{
    public class BloodInventoryRequest
    {
        [Required]
        public BloodType BloodType { get; set; }

        [Range(0, 1000)]
        public int UnitsAvailable { get; set; }
    }
}
