using System;

namespace BloodBond.DAL.DTO.Response
{
    public class BloodBankRatingResponse
    {
        public int Id { get; set; }
        public int BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
