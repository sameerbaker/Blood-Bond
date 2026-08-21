using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBloodDriveEventRepository : IGenericRepository<BloodDriveEvent>
    {
        Task<IEnumerable<BloodDriveEvent>> GetUpcomingAsync();
        Task<IEnumerable<BloodDriveEvent>> GetByBloodBankAsync(int bloodBankId);
        Task<BloodDriveEvent?> GetWithAttendancesAsync(int eventId);
    }
}
