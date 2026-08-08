using System;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Response
{
    public class BloodInventoryResponse
    {
        public int Id { get; set; }
        public int BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public BloodType BloodType { get; set; }
        public int UnitsAvailable { get; set; }
        public bool IsLowStock => UnitsAvailable < 5;
        public DateTime LastUpdated { get; set; }
    }
}
