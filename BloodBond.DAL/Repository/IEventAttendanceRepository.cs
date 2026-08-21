using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IEventAttendanceRepository : IGenericRepository<EventAttendance>
    {
        Task<EventAttendance?> GetAsync(int eventId, string userId);
        Task<IEnumerable<EventAttendance>> GetByEventAsync(int eventId);
        Task<IEnumerable<EventAttendance>> GetByUserAsync(string userId);
    }
}
