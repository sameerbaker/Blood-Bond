using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;

namespace BloodBond.DAL.Repository
{
    public interface IBloodInventoryRepository : IGenericRepository<BloodInventory>
    {
        Task<BloodInventory?> GetByBankAndTypeAsync(int bloodBankId, BloodType bloodType);
        Task<IEnumerable<BloodInventory>> GetLowStockAsync(int threshold = 5);
        Task<BloodInventory?> GetWithBankAsync(int inventoryId);
    }
}
