using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IBloodBankService
    {
        Task<BloodBankResponse> CreateAsync(string managerId, BloodBankRequest request);
        Task<BloodBankResponse?> GetByIdAsync(int id);
        Task<IEnumerable<BloodBankResponse>> GetAllAsync();
        Task<IEnumerable<BloodBankResponse>> GetVerifiedAsync();
        Task<BloodBankResponse?> GetMineAsync(string managerId);
        Task<BloodBankResponse> UpdateAsync(int id, string managerId, BloodBankRequest request);
        Task<BloodBankResponse> ApproveAsync(int id);    
        Task<BloodBankResponse> RejectAsync(int id);     
        Task<BloodBankResponse> SetInventoryAsync(int id, string managerId, List<BloodInventoryRequest> items);
        Task<IEnumerable<BloodInventoryResponse>> GetLowStockAsync();
    }
}
