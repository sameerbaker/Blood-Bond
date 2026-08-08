using System;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Response
{
    public class DonationResponse
    {
        public int Id { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public int BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public int? RequestId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? DonationDate { get; set; }
        public int UnitsDonated { get; set; }
        public DonationStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
