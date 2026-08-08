using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Enums;

namespace BloodBond.BLL.Service
{
    public interface IBloodRequestService
    {
        Task<BloodRequestResponse> CreateAsync(string requesterId, BloodRequestRequest request);
        Task<BloodRequestResponse?> GetByIdAsync(int id);
        Task<IEnumerable<BloodRequestResponse>> GetMineAsync(string requesterId);
        Task<IEnumerable<BloodRequestResponse>> GetActiveByCityAsync(string city);
        Task<BloodRequestResponse> CancelAsync(int id, string requesterId);
        Task<BloodRequestResponse> MarkInProgressAsync(int id);     
        Task<BloodRequestResponse> MarkFulfilledAsync(int id);
        Task<int> NotifyCompatibleDonorsAsync(int requestId);       
    }
}
