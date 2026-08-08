using System;

namespace BloodBond.DAL.DTO.Response
{
    public class EligibilityAnswerResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public double Weight { get; set; }
        public int Age { get; set; }
        public bool HasChronicDisease { get; set; }
        public DateTime? LastSurgeryDate { get; set; }
        public bool Passed { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
