using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
    }
}
