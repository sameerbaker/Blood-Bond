using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBloodBankRatingRepository : IGenericRepository<BloodBankRating>
    {
        Task<IEnumerable<BloodBankRating>> GetByBloodBankAsync(int bloodBankId);
        Task<BloodBankRating?> GetByUserAndBankAsync(string userId, int bloodBankId);
        Task<double> GetAverageRatingAsync(int bloodBankId);
        Task<int> GetCountAsync(int bloodBankId);
    }
}
