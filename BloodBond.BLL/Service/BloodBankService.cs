using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Enums;
using BloodBond.DAL.Models;
using BloodBond.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.BLL.Service
{
    public class BloodBankService : IBloodBankService
    {
        private readonly IBloodBankRepository _bankRepo;
        private readonly IBloodInventoryRepository _inventoryRepo;
        private readonly ApplicationDbContext _context;

        public BloodBankService(
            IBloodBankRepository bankRepo,
            IBloodInventoryRepository inventoryRepo,
            ApplicationDbContext context)
        {
            _bankRepo = bankRepo;
            _inventoryRepo = inventoryRepo;
            _context = context;
        }

        public async Task<BloodBankResponse> CreateAsync(string managerId, BloodBankRequest request)
        {
            var existing = await _bankRepo.GetByManagerIdAsync(managerId);
            if (existing != null)
                throw new InvalidOperationException("You already have a blood bank registered.");

            var bank = new BloodBank
            {
                Name = request.Name,
                CityAddress = request.CityAddress,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ContactPhone = request.ContactPhone,
                ManagerId = managerId,
                Status = BloodBankStatus.Pending,
                CreatedById = managerId
            };

            await _bankRepo.AddAsync(bank);
            await _context.SaveChangesAsync();

            return await MapToResponseAsync(bank);
        }

        public async Task<BloodBankResponse?> GetByIdAsync(int id)
        {
            var bank = await _bankRepo.GetByIdAsync(id);
            return bank == null ? null : await MapToResponseAsync(bank);
        }

        public async Task<IEnumerable<BloodBankResponse>> GetAllAsync()
        {
            var banks = await _context.BloodBanks
                .AsNoTracking()
                .Include(b => b.Manager)
                .Include(b => b.Inventory)
                .ToListAsync();
            return banks.Select(MapToResponseSimple);
        }

        public async Task<IEnumerable<BloodBankResponse>> GetVerifiedAsync()
        {
            var banks = await _bankRepo.GetByStatusAsync(BloodBankStatus.Verified);
            return banks.Select(MapToResponseSimple);
        }

        public async Task<BloodBankResponse?> GetMineAsync(string managerId)
        {
            var bank = await _bankRepo.GetByManagerIdAsync(managerId);
            return bank == null ? null : MapToResponseSimple(bank);
        }

        public async Task<BloodBankResponse> UpdateAsync(int id, string managerId, BloodBankRequest request)
        {
            var bank = await _bankRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            bank.Name = request.Name;
            bank.CityAddress = request.CityAddress;
            bank.Latitude = request.Latitude;
            bank.Longitude = request.Longitude;
            bank.ContactPhone = request.ContactPhone;
            bank.UpdatedById = managerId;

            _bankRepo.Update(bank);
            await _context.SaveChangesAsync();

            return MapToResponseSimple(bank);
        }

        public async Task<BloodBankResponse> ApproveAsync(int id)
        {
            var bank = await _bankRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            bank.Status = BloodBankStatus.Verified;
            _bankRepo.Update(bank);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(bank);
        }

        public async Task<BloodBankResponse> RejectAsync(int id)
        {
            var bank = await _bankRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            bank.Status = BloodBankStatus.Rejected;
            _bankRepo.Update(bank);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(bank);
        }

        public async Task<BloodBankResponse> SetInventoryAsync(int id, string managerId, List<BloodInventoryRequest> items)
        {
            var bank = await _bankRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            var existing = await _context.BloodInventories
                .Where(i => i.BloodBankId == id)
                .ToListAsync();
            _context.BloodInventories.RemoveRange(existing);

            foreach (var item in items)
            {
                _context.BloodInventories.Add(new BloodInventory
                {
                    BloodBankId = id,
                    BloodType = item.BloodType,
                    UnitsAvailable = item.UnitsAvailable,
                    LastUpdated = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            var updated = await _bankRepo.GetByIdAsync(id);
            return MapToResponseSimple(updated!);
        }

        public async Task<IEnumerable<BloodInventoryResponse>> GetLowStockAsync()
        {
            var items = await _inventoryRepo.GetLowStockAsync();
            return items.Select(i => new BloodInventoryResponse
            {
                Id = i.Id,
                BloodBankId = i.BloodBankId,
                BloodBankName = i.BloodBank?.Name,
                BloodType = i.BloodType,
                UnitsAvailable = i.UnitsAvailable,
                LastUpdated = i.LastUpdated
            });
        }

        private async Task<BloodBankResponse> MapToResponseAsync(BloodBank bank)
        {
            await Task.CompletedTask;
            return MapToResponseSimple(bank);
        }

        private static BloodBankResponse MapToResponseSimple(BloodBank bank)
        {
            return new BloodBankResponse
            {
                Id = bank.Id,
                Name = bank.Name,
                CityAddress = bank.CityAddress,
                Latitude = bank.Latitude,
                Longitude = bank.Longitude,
                ContactPhone = bank.ContactPhone,
                Status = bank.Status,
                ManagerId = bank.ManagerId,
                ManagerName = bank.Manager?.FullName,
                Inventory = bank.Inventory?.Select(i => new BloodInventoryResponse
                {
                    Id = i.Id,
                    BloodBankId = i.BloodBankId,
                    BloodType = i.BloodType,
                    UnitsAvailable = i.UnitsAvailable,
                    LastUpdated = i.LastUpdated
                }).ToList() ?? new(),
                CreatedAt = bank.CreatedAt
            };
        }
    }
}
