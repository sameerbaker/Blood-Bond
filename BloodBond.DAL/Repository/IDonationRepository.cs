using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IDonationRepository : IGenericRepository<Donation>
    {
        Task<IEnumerable<Donation>> GetByDonorAsync(string donorId);
        Task<IEnumerable<Donation>> GetByBloodBankAsync(int bloodBankId);
        Task<IEnumerable<Donation>> GetByStatusAsync(DonationStatus status);
    }
}
