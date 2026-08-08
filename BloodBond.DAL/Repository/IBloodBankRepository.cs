using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBloodBankRepository : IGenericRepository<BloodBank>
    {
        Task<IEnumerable<BloodBank>> GetByStatusAsync(BloodBankStatus status);
        Task<BloodBank?> GetByManagerIdAsync(string managerId);
        Task<IEnumerable<BloodBank>> GetVerifiedByCityAsync(string city);
    }
}
