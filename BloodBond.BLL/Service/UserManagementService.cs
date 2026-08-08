using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BloodBond.BLL.Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        private static readonly string[] ValidRoles = { "User", "BloodBankManager", "Admin" };

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        // -------- Bootstrap (no admin required) --------
        public async Task<RegisterResponse> RegisterFirstAdminAsync(RegisterFirstAdminRequest request)
        {
            var configuredKey = _configuration["AdminBootstrap:SecretKey"];
            if (string.IsNullOrWhiteSpace(configuredKey))
                throw new InvalidOperationException(
                    "AdminBootstrap:SecretKey is not configured. Set it in appsettings or environment variables.");
            if (request.SecretKey != configuredKey)
                throw new UnauthorizedAccessException("Invalid secret key.");

            var anyAdmin = (await _userManager.GetUsersInRoleAsync("Admin")).Any();
            if (anyAdmin)
                throw new InvalidOperationException(
                    "An admin already exists. Use POST /api/admin/create to add more admins.");

            // 1. Create user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Admin");

            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = new[] { "Admin" }
            };
        }

        // -------- Admin-only --------
        public async Task<RegisterResponse> CreateUserAsync(CreateUserByAdminRequest request)
        {
            if (!ValidRoles.Contains(request.Role))
                throw new ArgumentException($"Role must be one of: {string.Join(", ", ValidRoles)}");

            if (request.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, request.Role);

            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = new[] { request.Role }
            };
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException("New password and confirmation do not match.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<IEnumerable<UserListResponse>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            var result = new List<UserListResponse>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserListResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber,
                    IsBlocked = u.IsBlocked,
                    CreatedAt = u.CreatedAt,
                    Roles = roles.ToArray()
                });
            }
            return result;
        }

        public async Task<UserListResponse?> GetUserByIdAsync(string userId)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return null;
            var roles = await _userManager.GetRolesAsync(u);
            return new UserListResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber,
                IsBlocked = u.IsBlocked,
                CreatedAt = u.CreatedAt,
                Roles = roles.ToArray()
            };
        }

        public async Task<UserListResponse> BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");
            user.IsBlocked = true;
            await _userManager.UpdateAsync(user);
            return (await GetUserByIdAsync(userId))!;
        }

        public async Task<UserListResponse> UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");
            user.IsBlocked = false;
            await _userManager.UpdateAsync(user);
            return (await GetUserByIdAsync(userId))!;
        }

        public async Task<UserListResponse> ChangeRoleAsync(string userId, ChangeRoleRequest request)
        {
            if (!ValidRoles.Contains(request.Role))
                throw new ArgumentException($"Role must be one of: {string.Join(", ", ValidRoles)}");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, request.Role);

            return (await GetUserByIdAsync(userId))!;
        }
    }
}
