using System;

namespace BloodBond.DAL.DTO.Response
{
    public class MonetaryDonationResponse
    {
        public int Id { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public int? BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public DateTime DonationDate { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
