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
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepo;
        private readonly IBloodBankRepository _bankRepo;
        private readonly IBloodInventoryRepository _inventoryRepo;
        private readonly ApplicationDbContext _context;

        public DonationService(
            IDonationRepository donationRepo,
            IBloodBankRepository bankRepo,
            IBloodInventoryRepository inventoryRepo,
            ApplicationDbContext context)
        {
            _donationRepo = donationRepo;
            _bankRepo = bankRepo;
            _inventoryRepo = inventoryRepo;
            _context = context;
        }

        public async Task<DonationResponse> ScheduleAsync(string donorId, DonationRequest request)
        {
            var bank = await _bankRepo.GetByIdAsync(request.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");

            if (bank.Status != BloodBankStatus.Verified)
                throw new InvalidOperationException("You can only schedule at verified blood banks.");

            var donation = new Donation
            {
                DonorId = donorId,
                BloodBankId = request.BloodBankId,
                RequestId = request.RequestId,
                ScheduledDate = request.ScheduledDate,
                Notes = request.Notes,
                Status = DonationStatus.Scheduled
            };

            await _donationRepo.AddAsync(donation);
            await _context.SaveChangesAsync();

            return MapToResponseSimple(donation);
        }

        public async Task<DonationResponse> ApproveAsync(int id, string managerId)
        {
            var donation = await _donationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Donation not found.");

            await EnsureManagerOwnsBankAsync(donation.BloodBankId, managerId);

            donation.Status = DonationStatus.Approved;
            _donationRepo.Update(donation);
            await _context.SaveChangesAsync();

            if (donation.RequestId.HasValue)
            {
                var request = await _context.BloodRequests.FindAsync(donation.RequestId.Value);
                if (request != null && request.Status == RequestStatus.Pending)
                {
                    request.Status = RequestStatus.InProgress;
                    _context.BloodRequests.Update(request);
                    await _context.SaveChangesAsync();
                }
            }

            return MapToResponseSimple(donation);
        }

        public async Task<DonationResponse> RejectAsync(int id, string managerId, string? notes)
        {
            var donation = await _donationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Donation not found.");

            await EnsureManagerOwnsBankAsync(donation.BloodBankId, managerId);

            donation.Status = DonationStatus.Rejected;
            donation.Notes = notes ?? donation.Notes;
            _donationRepo.Update(donation);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(donation);
        }

        public async Task<DonationResponse> CompleteAsync(int id, string managerId, CompleteDonationRequest request)
        {
            var donation = await _donationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Donation not found.");

            await EnsureManagerOwnsBankAsync(donation.BloodBankId, managerId);

            if (donation.Status != DonationStatus.Approved && donation.Status != DonationStatus.Scheduled)
                throw new InvalidOperationException("Only approved or scheduled donations can be completed.");

            donation.Status = DonationStatus.Completed;
            donation.DonationDate = DateTime.UtcNow;
            donation.UnitsDonated = request.UnitsDonated;
            donation.Notes = request.Notes ?? donation.Notes;

            _donationRepo.Update(donation);

            var donor = await _context.Users.FindAsync(donation.DonorId);
            if (donor?.BloodType.HasValue == true)
            {
                var inv = await _inventoryRepo.GetByBankAndTypeAsync(donation.BloodBankId, donor.BloodType.Value);
                if (inv == null)
                {
                    _context.BloodInventories.Add(new BloodInventory
                    {
                        BloodBankId = donation.BloodBankId,
                        BloodType = donor.BloodType.Value,
                        UnitsAvailable = request.UnitsDonated,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    inv.UnitsAvailable += request.UnitsDonated;
                    inv.LastUpdated = DateTime.UtcNow;
                    _inventoryRepo.Update(inv);
                }
            }

            if (donor != null)
            {
                donor.LastDonationDate = DateTime.UtcNow;
                donor.Points += 10; // 10 points per donation
                _context.Users.Update(donor);
            }

            
            if (donation.RequestId.HasValue)
            {
                var linkedRequest = await _context.BloodRequests
                    .Include(r => r.Requester)
                    .FirstOrDefaultAsync(r => r.Id == donation.RequestId.Value);
                if (linkedRequest != null)
                {
                    linkedRequest.Status = RequestStatus.Fulfilled;
                    _context.BloodRequests.Update(linkedRequest);
                }
            }

            await _context.SaveChangesAsync();
            return MapToResponseSimple(donation);
        }

        public async Task<DonationResponse> CancelAsync(int id, string donorId)
        {
            var donation = await _donationRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Donation not found.");

            if (donation.DonorId != donorId)
                throw new UnauthorizedAccessException("You can only cancel your own donation.");

            donation.Status = DonationStatus.Cancelled;
            _donationRepo.Update(donation);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(donation);
        }

        public async Task<DonationResponse?> GetByIdAsync(int id)
        {
            var d = await _donationRepo.GetByIdAsync(id);
            return d == null ? null : MapToResponseSimple(d);
        }

        public async Task<IEnumerable<DonationResponse>> GetByDonorAsync(string donorId)
        {
            var list = await _donationRepo.GetByDonorAsync(donorId);
            return list.Select(MapToResponseSimple);
        }

        public async Task<IEnumerable<DonationResponse>> GetByBloodBankAsync(int bloodBankId, string managerId)
        {
            await EnsureManagerOwnsBankAsync(bloodBankId, managerId);
            var list = await _donationRepo.GetByBloodBankAsync(bloodBankId);
            return list.Select(MapToResponseSimple);
        }

        private async Task EnsureManagerOwnsBankAsync(int bloodBankId, string managerId)
        {
            var bank = await _bankRepo.GetByIdAsync(bloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");
        }

        private static DonationResponse MapToResponseSimple(Donation d) => new()
        {
            Id = d.Id,
            DonorId = d.DonorId,
            BloodBankId = d.BloodBankId,
            RequestId = d.RequestId,
            ScheduledDate = d.ScheduledDate,
            DonationDate = d.DonationDate,
            UnitsDonated = d.UnitsDonated,
            Status = d.Status,
            Notes = d.Notes
        };
    }
}
