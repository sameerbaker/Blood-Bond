using System;
using System.Collections.Generic;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Response
{
    public class BloodBankResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CityAddress { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ContactPhone { get; set; }
        public BloodBankStatus Status { get; set; }
        public string ManagerId { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public List<BloodInventoryResponse> Inventory { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
