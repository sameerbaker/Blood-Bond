using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBond.BLL.Service;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodBond.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserManagementService _userService;

        public AdminController(IUserManagementService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Bootstrap the FIRST admin. Refuses if any admin already exists.
        /// Use this once when setting up a fresh database.
        /// </summary>
        [HttpPost("register-first")]
        [AllowAnonymous]
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
    }
}
