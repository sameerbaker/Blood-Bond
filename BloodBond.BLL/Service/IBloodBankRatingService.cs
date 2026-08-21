using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IBloodBankRatingService
    {
        Task<BloodBankRatingResponse> AddOrUpdateAsync(string userId, AddRatingRequest request);
        Task<IEnumerable<BloodBankRatingResponse>> GetByBloodBankAsync(int bloodBankId);
        Task<BloodBankRatingResponse?> GetByUserAndBankAsync(string userId, int bloodBankId);
        Task<BloodBankRatingStatsResponse> GetStatsAsync(int bloodBankId);
    }

    public class BloodBankRatingStatsResponse
    {
        public int BloodBankId { get; set; }
        public string? BloodBankName { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}
