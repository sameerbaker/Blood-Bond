using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBond.BLL.Service;
using BloodBond.DAL.Data;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using BloodBond.Resources;

namespace BloodBond.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<BloodBond.Resources.SharedResource> _localizer;

        public AdminController(
            IUserManagementService userService,
            ApplicationDbContext context,
            IStringLocalizer<BloodBond.Resources.SharedResource> localizer)
        {
            _userService = userService;
            _context = context;
            _localizer = localizer;
        }

        
        /// Bootstrap the FIRST admin. Refuses if any admin already exists.
        /// Use this once when setting up a fresh database.
        
        [HttpPost("register-first")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<ActionResult<RegisterResponse>> RegisterFirstAdmin(
            [FromBody] RegisterFirstAdminRequest request)
        {
            var result = await _userService.RegisterFirstAdminAsync(request);
            return Ok(result);
        }

        /// <summary>Create a new user (Admin only).</summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RegisterResponse>> CreateUser(
            [FromBody] CreateUserByAdminRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return Ok(result);
        }

        /// <summary>Change the current admin's own password.</summary>
        [HttpPost("change-password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _userService.ChangePasswordAsync(userId!, request);
            return Ok(new { message = "Password changed successfully." });
        }

        /// <summary>List all users (Admin only).</summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserListResponse>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>Get a specific user by id (Admin only).</summary>
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserListResponse>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>Block a user (Admin only).</summary>
        [HttpPatch("users/{id}/block")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserListResponse>> BlockUser(string id)
        {
            var user = await _userService.BlockUserAsync(id);
            return Ok(user);
        }

        /// <summary>Unblock a user (Admin only).</summary>
        [HttpPatch("users/{id}/unblock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserListResponse>> UnblockUser(string id)
        {
            var user = await _userService.UnblockUserAsync(id);
            return Ok(user);
        }

        /// <summary>Change a user's role (Admin only).</summary>
        [HttpPatch("users/{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserListResponse>> ChangeRole(string id, [FromBody] ChangeRoleRequest request)
        {
            var user = await _userService.ChangeRoleAsync(id, request);
            return Ok(user);
        }

        /// <summary>System-wide analytics dashboard (Admin only).</summary>
        [HttpGet("analytics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAnalytics()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalBloodBanks = await _context.BloodBanks.CountAsync();
            var verifiedBloodBanks = await _context.BloodBanks.CountAsync(b => b.Status == BloodBankStatus.Verified);
            var pendingBloodBanks = await _context.BloodBanks.CountAsync(b => b.Status == BloodBankStatus.Pending);

            var totalRequests = await _context.BloodRequests.CountAsync();
            var pendingRequests = await _context.BloodRequests.CountAsync(r => r.Status == RequestStatus.Pending);
            var fulfilledRequests = await _context.BloodRequests.CountAsync(r => r.Status == RequestStatus.Fulfilled);
            var criticalRequests = await _context.BloodRequests.CountAsync(r => r.UrgencyLevel == UrgencyLevel.Critical && r.Status == RequestStatus.Pending);

            var totalDonations = await _context.Donations.CountAsync();
            var completedDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Completed);

            var totalMonetary = await _context.MonetaryDonations
                .Where(m => m.Status == "Succeeded")
                .SumAsync(m => (decimal?)m.Amount) ?? 0m;

            // Blood type distribution
            var bloodTypeDistribution = await _context.Users
                .Where(u => u.BloodType.HasValue)
                .GroupBy(u => u.BloodType!.Value)
                .Select(g => new { BloodType = g.Key, Count = g.Count() })
                .ToListAsync();

            // Low stock items
            var lowStockCount = await _context.BloodInventories
                .CountAsync(i => i.UnitsAvailable < 5);

            return Ok(new
            {
                Users = new { Total = totalUsers },
                BloodBanks = new
                {
                    Total = totalBloodBanks,
                    Verified = verifiedBloodBanks,
                    Pending = pendingBloodBanks
                },
                Requests = new
                {
                    Total = totalRequests,
                    Pending = pendingRequests,
                    Fulfilled = fulfilledRequests,
                    CriticalPending = criticalRequests
                },
                Donations = new
                {
                    Total = totalDonations,
                    Completed = completedDonations
                },
                Monetary = new
                {
                    TotalDonatedUSD = totalMonetary
                },
                Inventory = new
                {
                    LowStockItems = lowStockCount
                },
                BloodTypeDistribution = bloodTypeDistribution
            });
        }

        /// <summary>
        /// Returns a sample of localized messages for the current culture.
        /// Use the ?lang=ar or ?lang=en query string (or the Accept-Language header)
        /// to switch language.
        /// </summary>
        [HttpGet("localization-sample")]
        [AllowAnonymous]
        public ActionResult GetLocalizationSample()
        {
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            string Resolve(string key) =>
                _localizer[key].ResourceNotFound ? key : _localizer[key].Value;

            return Ok(new
            {
                CurrentCulture = currentCulture,
                Welcome = Resolve("Welcome"),
                UserRegistered = Resolve("UserRegistered"),
                EligibilityPassed = Resolve("EligibilityPassed"),
                DonationCompleted = Resolve("DonationCompleted"),
                RateLimitExceeded = Resolve("RateLimitExceeded"),
                BloodTypeAPpositive = Resolve("BloodTypeAPpositive"),
                UrgencyLevelCritical = Resolve("UrgencyLevelCritical")
            });
        }
    }
}
