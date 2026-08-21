using System;
using System.Collections.Generic;
using BloodBond.DAL.Enums;

namespace BloodBond.DAL.DTO.Response
{
    public class BloodDriveEventResponse
    {
        public int Id { get; set; }
        public int BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventAttendanceResponse
    {
        public int EventId { get; set; }
        public string? EventTitle { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public CheckInStatus Status { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
    }
}
