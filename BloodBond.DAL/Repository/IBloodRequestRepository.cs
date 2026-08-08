using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBloodRequestRepository : IGenericRepository<BloodRequest>
    {
        Task<IEnumerable<BloodRequest>> GetByRequesterAsync(string requesterId);
        Task<IEnumerable<BloodRequest>> GetActiveByCityAsync(string city);
        Task<IEnumerable<BloodRequest>> GetByUrgencyAsync(UrgencyLevel level);
    }
}
