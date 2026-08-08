using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBond.BLL.Service;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloodBond.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodBanksController : ControllerBase
    {
        private readonly IBloodBankService _bankService;
        private readonly UserManager<BloodBond.DAL.Models.ApplicationUser> _userManager;

        public BloodBanksController(IBloodBankService bankService,
            UserManager<BloodBond.DAL.Models.ApplicationUser> userManager)
        {
            _bankService = bankService;
            _userManager = userManager;
        }

        // Public: list all verified blood banks
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodBankResponse>>> GetAll()
        {
            var banks = await _bankService.GetAllAsync();
            return Ok(banks);
        }

        [HttpGet("verified")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodBankResponse>>> GetVerified()
        {
            var banks = await _bankService.GetVerifiedAsync();
            return Ok(banks);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BloodBankResponse>> GetById(int id)
        {
            var bank = await _bankService.GetByIdAsync(id);
            if (bank == null) return NotFound();
            return Ok(bank);
        }

        // BloodBankManager: register a blood bank
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<BloodBankResponse>> Create([FromBody] BloodBankRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Promote to BloodBankManager role on first bank
            var user = await _userManager.FindByIdAsync(userId!);
            if (user !=null && !await _userManager.IsInRoleAsync(user, "BloodBankManager")
                             && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "BloodBankManager");
            }
            var bank = await _bankService.CreateAsync(userId!, request);
            return Ok(bank);
        }

        // BloodBankManager: my bank
        [HttpGet("mine")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodBankResponse>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bank = await _bankService.GetMineAsync(userId!);
            if (bank == null) return NotFound("You don't have a registered blood bank yet.");
            return Ok(bank);
        }

        // BloodBankManager: update own bank
        [HttpPut("{id}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodBankResponse>> Update(int id, [FromBody] BloodBankRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bank = await _bankService.UpdateAsync(id, userId!, request);
            return Ok(bank);
        }

        // BloodBankManager: set inventory
        [HttpPut("{id}/inventory")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodBankResponse>> SetInventory(int id, [FromBody] List<BloodInventoryRequest> items)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bank = await _bankService.SetInventoryAsync(id, userId!, items);
            return Ok(bank);
        }

        // Admin: approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BloodBankResponse>> Approve(int id)
        {
            var bank = await _bankService.ApproveAsync(id);
            return Ok(bank);
        }

        // Admin: reject
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BloodBankResponse>> Reject(int id)
        {
            var bank = await _bankService.RejectAsync(id);
            return Ok(bank);
        }

        // Public: low stock alerts
        [HttpGet("low-stock")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodInventoryResponse>>> GetLowStock()
        {
            var items = await _bankService.GetLowStockAsync();
            return Ok(items);
        }
    }
}
