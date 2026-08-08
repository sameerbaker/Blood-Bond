using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IEligibilityService
    {
        Task<EligibilityAnswerResponse> CheckAsync(string userId, EligibilityAnswerRequest request);
        Task<EligibilityAnswerResponse?> GetLatestAsync(string userId);
    }
}
