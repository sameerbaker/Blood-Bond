using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class MonetaryDonationRequest
    {
        [Required, Range(0.01, 1_000_000)]
        public decimal Amount { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "usd";

        public int? BloodBankId { get; set; }
    }
}
