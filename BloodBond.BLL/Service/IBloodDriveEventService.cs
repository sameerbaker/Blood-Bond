using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IBloodDriveEventService
    {
        Task<BloodDriveEventResponse> CreateAsync(string managerId, BloodDriveEventRequest request);
        Task<BloodDriveEventResponse?> GetByIdAsync(int id);
        Task<IEnumerable<BloodDriveEventResponse>> GetUpcomingAsync();
        Task<IEnumerable<BloodDriveEventResponse>> GetByBloodBankAsync(int bloodBankId, string managerId);
        Task<BloodDriveEventResponse> UpdateAsync(int id, string managerId, BloodDriveEventRequest request);
        Task DeleteAsync(int id, string managerId);

        
        Task<EventAttendanceResponse> RegisterAsync(int eventId, string userId);
        Task<EventAttendanceResponse> CheckInAsync(int eventId, string userId);
        Task<EventAttendanceResponse> CancelAsync(int eventId, string userId);
        Task<IEnumerable<EventAttendanceResponse>> GetAttendancesAsync(int eventId, string managerId);
        Task<IEnumerable<EventAttendanceResponse>> GetMyEventsAsync(string userId);
    }
}
