using System;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Response
{
    public class BloodRequestResponse
    {
        public int Id { get; set; }
        public string RequesterId { get; set; } = string.Empty;
        public string? RequesterName { get; set; }
        public BloodType BloodType { get; set; }
        public int UnitsNeeded { get; set; }
        public UrgencyLevel UrgencyLevel { get; set; }
        public RequestStatus Status { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
