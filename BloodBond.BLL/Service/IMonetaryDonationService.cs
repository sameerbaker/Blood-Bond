using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IMonetaryDonationService
    {
        
        Task<PaymentIntentResponse> CreatePaymentIntentAsync(string donorId, MonetaryDonationRequest request);

        Task<MonetaryDonationResponse> ConfirmDonationAsync(string paymentIntentId, string status);

        Task<IEnumerable<MonetaryDonationResponse>> GetMineAsync(string donorId);

        Task<IEnumerable<MonetaryDonationResponse>> GetByBloodBankAsync(int bloodBankId, string managerId);

        /// <summary>Total money donated by a user.</summary>
        Task<decimal> GetTotalByDonorAsync(string donorId);

        /// <summary>Admin only — list every monetary donation in the system.</summary>
        Task<IEnumerable<MonetaryDonationResponse>> GetAllAsync();

        /// <summary>Bank manager / admin — list monetary donations directed at the given bank.</summary>
        Task<IEnumerable<MonetaryDonationResponse>> GetByBankAsync(int bankId, string managerId);
    }
}
