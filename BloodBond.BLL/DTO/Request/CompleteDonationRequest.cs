using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class CompleteDonationRequest
    {
        [Range(0, 10)]
        public int UnitsDonated { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
