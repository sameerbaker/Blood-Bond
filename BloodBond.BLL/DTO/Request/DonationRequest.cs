using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class DonationRequest
    {
        [Required]
        public int BloodBankId { get; set; }

        public int? RequestId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
