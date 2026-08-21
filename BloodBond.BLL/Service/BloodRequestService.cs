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
using BloodBond.DAL.utils;
using Microsoft.EntityFrameworkCore;

namespace BloodBond.BLL.Service
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _requestRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly ApplicationDbContext _context;

        public BloodRequestService(
            IBloodRequestRepository requestRepo,
            INotificationRepository notificationRepo,
            ApplicationDbContext context)
        {
            _requestRepo = requestRepo;
            _notificationRepo = notificationRepo;
            _context = context;
        }

        public async Task<BloodRequestResponse> CreateAsync(string requesterId, BloodRequestRequest request)
        {
            var entity = new BloodRequest
            {
                RequesterId = requesterId,
                BloodType = request.BloodType,
                UnitsNeeded = request.UnitsNeeded,
                UrgencyLevel = request.UrgencyLevel,
                City = request.City,
                ExpiryDate = request.ExpiryDate,
                Notes = request.Notes,
                Status = RequestStatus.Pending
            };

            await _requestRepo.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Don't fail the request if notification dispatch has an issue
            try
            {
                await NotifyCompatibleDonorsAsync(entity.Id);
            }
            catch
            {
                // Swallow — request itself was created successfully
            }

            return await MapToResponseAsync(entity);
        }

        public async Task<BloodRequestResponse?> GetByIdAsync(int id)
        {
            var entity = await _requestRepo.GetByIdAsync(id);
            return entity == null ? null : MapToResponseSimple(entity);
        }

        public async Task<IEnumerable<BloodRequestResponse>> GetMineAsync(string requesterId)
        {
            var list = await _requestRepo.GetByRequesterAsync(requesterId);
            return list.Select(MapToResponseSimple);
        }

        public async Task<IEnumerable<BloodRequestResponse>> GetActiveByCityAsync(string city)
        {
            var list = await _requestRepo.GetActiveByCityAsync(city);
            return list.Select(MapToResponseSimple);
        }

        public async Task<BloodRequestResponse> CancelAsync(int id, string requesterId)
        {
            var entity = await _requestRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Request not found.");

            if (entity.RequesterId != requesterId)
                throw new UnauthorizedAccessException("You are not the requester.");

            entity.Status = RequestStatus.Cancelled;
            _requestRepo.Update(entity);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(entity);
        }

        public async Task<BloodRequestResponse> MarkInProgressAsync(int id)
        {
            var entity = await _requestRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Request not found.");

            if (entity.Status == RequestStatus.Pending)
                entity.Status = RequestStatus.InProgress;

            _requestRepo.Update(entity);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(entity);
        }

        public async Task<BloodRequestResponse> MarkFulfilledAsync(int id)
        {
            var entity = await _requestRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Request not found.");

            entity.Status = RequestStatus.Fulfilled;
            _requestRepo.Update(entity);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(entity);
        }

        public async Task<int> NotifyCompatibleDonorsAsync(int requestId)
        {
            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) return 0;

            // Filter what we can in SQL, then apply compatibility in memory
            var candidates = await _context.Users
                .AsNoTracking()
                .Where(u => u.BloodType.HasValue
                            && u.City == request.City
                            && !u.IsBlocked)
                .ToListAsync();

            var compatibleDonors = candidates
                .Where(u => BloodCompatibility.CanDonateTo(u.BloodType!.Value, request.BloodType))
                .ToList();

            var notifType = request.UrgencyLevel == UrgencyLevel.Critical ? "Emergency" : "Request";

            foreach (var donor in compatibleDonors)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = donor.Id,
                    Type = notifType,
                    Message = request.UrgencyLevel == UrgencyLevel.Critical
                        ? $"🚨 CRITICAL: Blood type {request.BloodType} needed in {request.City} ({request.UnitsNeeded} units)"
                        : $"New blood request: {request.BloodType} in {request.City} ({request.UnitsNeeded} units)"
                });
            }

            await _context.SaveChangesAsync();
            return compatibleDonors.Count;
        }

        private async Task<BloodRequestResponse> MapToResponseAsync(BloodRequest entity)
        {
            await Task.CompletedTask;
            return MapToResponseSimple(entity);
        }

        private static BloodRequestResponse MapToResponseSimple(BloodRequest r) => new()
        {
            Id = r.Id,
            RequesterId = r.RequesterId,
            BloodType = r.BloodType,
            UnitsNeeded = r.UnitsNeeded,
            UrgencyLevel = r.UrgencyLevel,
            Status = r.Status,
            City = r.City,
            ExpiryDate = r.ExpiryDate,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        };
    }
}
