using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class BloodDriveEventRequest
    {
        [Required]
        public int BloodBankId { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 10000)]
        public int Capacity { get; set; } = 50;
    }
}
