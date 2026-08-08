using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IDonationService
    {
        Task<DonationResponse> ScheduleAsync(string donorId, DonationRequest request);
        Task<DonationResponse> ApproveAsync(int id, string managerId);
        Task<DonationResponse> RejectAsync(int id, string managerId, string? notes);
        Task<DonationResponse> CompleteAsync(int id, string managerId, CompleteDonationRequest request);
        Task<DonationResponse> CancelAsync(int id, string donorId);
        Task<DonationResponse?> GetByIdAsync(int id);
        Task<IEnumerable<DonationResponse>> GetByDonorAsync(string donorId);
        Task<IEnumerable<DonationResponse>> GetByBloodBankAsync(int bloodBankId, string managerId);
    }
}
