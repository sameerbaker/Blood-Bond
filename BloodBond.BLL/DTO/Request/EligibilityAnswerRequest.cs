using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class EligibilityAnswerRequest
    {
        [Range(30, 300)]
        public double Weight { get; set; }

        [Range(16, 100)]
        public int Age { get; set; }

        public bool HasChronicDisease { get; set; }

        public DateTime? LastSurgeryDate { get; set; }
    }
}
