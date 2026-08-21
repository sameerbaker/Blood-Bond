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
    public class BloodDriveEventService : IBloodDriveEventService
    {
        private readonly IBloodDriveEventRepository _eventRepo;
        private readonly IEventAttendanceRepository _attendanceRepo;
        private readonly IBloodBankRepository _bankRepo;
        private readonly ApplicationDbContext _context;

        public BloodDriveEventService(
            IBloodDriveEventRepository eventRepo,
            IEventAttendanceRepository attendanceRepo,
            IBloodBankRepository bankRepo,
            ApplicationDbContext context)
        {
            _eventRepo = eventRepo;
            _attendanceRepo = attendanceRepo;
            _bankRepo = bankRepo;
            _context = context;
        }

        public async Task<BloodDriveEventResponse> CreateAsync(string managerId, BloodDriveEventRequest request)
        {
            var bank = await _bankRepo.GetByIdAsync(request.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            var ev = new BloodDriveEvent
            {
                BloodBankId = request.BloodBankId,
                Title = request.Title,
                Location = request.Location,
                EventDate = request.EventDate,
                Description = request.Description,
                Capacity = request.Capacity
            };
            await _eventRepo.AddAsync(ev);
            await _context.SaveChangesAsync();
            return await MapToResponseAsync(ev);
        }

        public async Task<BloodDriveEventResponse?> GetByIdAsync(int id)
        {
            var ev = await _eventRepo.GetWithAttendancesAsync(id);
            return ev == null ? null : MapToResponseSimple(ev);
        }

        public async Task<IEnumerable<BloodDriveEventResponse>> GetUpcomingAsync()
        {
            var events = await _eventRepo.GetUpcomingAsync();
            var result = new List<BloodDriveEventResponse>();
            foreach (var e in events)
            {
                var count = await _context.EventAttendances.CountAsync(a => a.EventId == e.Id);
                result.Add(MapToResponseSimple(e, count));
            }
            return result;
        }

        public async Task<IEnumerable<BloodDriveEventResponse>> GetByBloodBankAsync(int bloodBankId, string managerId)
        {
            var bank = await _bankRepo.GetByIdAsync(bloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            var events = await _eventRepo.GetByBloodBankAsync(bloodBankId);
            var result = new List<BloodDriveEventResponse>();
            foreach (var e in events)
            {
                var count = await _context.EventAttendances.CountAsync(a => a.EventId == e.Id);
                result.Add(MapToResponseSimple(e, count));
            }
            return result;
        }

        public async Task<BloodDriveEventResponse> UpdateAsync(int id, string managerId, BloodDriveEventRequest request)
        {
            var ev = await _eventRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Event not found.");

            var bank = await _bankRepo.GetByIdAsync(ev.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            ev.Title = request.Title;
            ev.Location = request.Location;
            ev.EventDate = request.EventDate;
            ev.Description = request.Description;
            ev.Capacity = request.Capacity;
            _eventRepo.Update(ev);
            await _context.SaveChangesAsync();
            return MapToResponseSimple(ev);
        }

        public async Task DeleteAsync(int id, string managerId)
        {
            var ev = await _eventRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Event not found.");
            var bank = await _bankRepo.GetByIdAsync(ev.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");
            _eventRepo.Delete(ev);
            await _context.SaveChangesAsync();
        }

        public async Task<EventAttendanceResponse> RegisterAsync(int eventId, string userId)
        {
            var ev = await _eventRepo.GetByIdAsync(eventId)
                ?? throw new KeyNotFoundException("Event not found.");

            var existing = await _attendanceRepo.GetAsync(eventId, userId);
            if (existing != null)
                throw new InvalidOperationException("You are already registered for this event.");

            // Check capacity
            var currentCount = await _context.EventAttendances.CountAsync(a => a.EventId == eventId);
            if (currentCount >= ev.Capacity)
                throw new InvalidOperationException("Event is at full capacity.");

            var attendance = new EventAttendance
            {
                EventId = eventId,
                UserId = userId,
                Status = CheckInStatus.Registered
            };
            await _attendanceRepo.AddAsync(attendance);
            await _context.SaveChangesAsync();

            return new EventAttendanceResponse
            {
                EventId = eventId,
                EventTitle = ev.Title,
                UserId = userId,
                Status = attendance.Status,
                RegisteredAt = attendance.RegisteredAt
            };
        }

        public async Task<EventAttendanceResponse> CheckInAsync(int eventId, string userId)
        {
            var attendance = await _attendanceRepo.GetAsync(eventId, userId)
                ?? throw new KeyNotFoundException("You are not registered for this event.");

            attendance.Status = CheckInStatus.CheckedIn;
            attendance.CheckedInAt = DateTime.UtcNow;
            _attendanceRepo.Update(attendance);
            await _context.SaveChangesAsync();

            var ev = await _eventRepo.GetByIdAsync(eventId);
            return new EventAttendanceResponse
            {
                EventId = eventId,
                EventTitle = ev?.Title,
                UserId = userId,
                Status = attendance.Status,
                RegisteredAt = attendance.RegisteredAt,
                CheckedInAt = attendance.CheckedInAt
            };
        }

        public async Task<EventAttendanceResponse> CancelAsync(int eventId, string userId)
        {
            var attendance = await _attendanceRepo.GetAsync(eventId, userId)
                ?? throw new KeyNotFoundException("You are not registered for this event.");

            attendance.Status = CheckInStatus.Cancelled;
            _attendanceRepo.Update(attendance);
            await _context.SaveChangesAsync();
            return new EventAttendanceResponse
            {
                EventId = eventId,
                UserId = userId,
                Status = attendance.Status,
                RegisteredAt = attendance.RegisteredAt
            };
        }

        public async Task<IEnumerable<EventAttendanceResponse>> GetAttendancesAsync(int eventId, string managerId)
        {
            var ev = await _eventRepo.GetByIdAsync(eventId)
                ?? throw new KeyNotFoundException("Event not found.");
            var bank = await _bankRepo.GetByIdAsync(ev.BloodBankId)
                ?? throw new KeyNotFoundException("Blood bank not found.");
            if (bank.ManagerId != managerId)
                throw new UnauthorizedAccessException("You are not the manager of this blood bank.");

            var list = await _attendanceRepo.GetByEventAsync(eventId);
            return list.Select(a => new EventAttendanceResponse
            {
                EventId = a.EventId,
                UserId = a.UserId,
                UserName = a.User?.FullName,
                Status = a.Status,
                RegisteredAt = a.RegisteredAt,
                CheckedInAt = a.CheckedInAt
            });
        }

        public async Task<IEnumerable<EventAttendanceResponse>> GetMyEventsAsync(string userId)
        {
            var list = await _attendanceRepo.GetByUserAsync(userId);
            return list.Select(a => new EventAttendanceResponse
            {
                EventId = a.EventId,
                EventTitle = a.Event?.Title,
                UserId = a.UserId,
                Status = a.Status,
                RegisteredAt = a.RegisteredAt,
                CheckedInAt = a.CheckedInAt
            });
        }

        private async Task<BloodDriveEventResponse> MapToResponseAsync(BloodDriveEvent e)
        {
            await Task.CompletedTask;
            return MapToResponseSimple(e);
        }

        private static BloodDriveEventResponse MapToResponseSimple(BloodDriveEvent e, int registeredCount = 0) => new()
        {
            Id = e.Id,
            BloodBankId = e.BloodBankId,
            BloodBankName = e.BloodBank?.Name,
            Title = e.Title,
            Location = e.Location,
            EventDate = e.EventDate,
            Description = e.Description,
            Capacity = e.Capacity,
            RegisteredCount = registeredCount,
            CreatedAt = e.CreatedAt
        };
    }
}
