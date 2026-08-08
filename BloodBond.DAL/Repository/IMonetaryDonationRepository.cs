using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IMonetaryDonationRepository : IGenericRepository<MonetaryDonation>
    {
        Task<IEnumerable<MonetaryDonation>> GetByDonorAsync(string donorId);
        Task<IEnumerable<MonetaryDonation>> GetByBloodBankAsync(int bloodBankId);
        Task<decimal> GetTotalByDonorAsync(string donorId);
    }
}
