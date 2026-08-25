using System.Collections.Generic;
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
    [Authorize]
    public class BloodRequestsController : ControllerBase
    {
        private readonly IBloodRequestService _requestService;

        public BloodRequestsController(IBloodRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<ActionResult<BloodRequestResponse>> Create([FromBody] BloodRequestRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _requestService.CreateAsync(userId!, request);
            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<BloodRequestResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _requestService.GetMineAsync(userId!);
            return Ok(list);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodRequestResponse>>> GetActive([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("city is required");
            var list = await _requestService.GetActiveByCityAsync(city);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BloodRequestResponse>> GetById(int id)
        {
            var result = await _requestService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<BloodRequestResponse>> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _requestService.CancelAsync(id, userId!);
            return Ok(result);
        }

        [HttpPatch("{id}/fulfill")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodRequestResponse>> Fulfill(int id)
        {
            var result = await _requestService.MarkFulfilledAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/notify")]
        [Authorize(Roles = "Admin,BloodBankManager")]
        public async Task<ActionResult> Notify(int id)
        {
            var count = await _requestService.NotifyCompatibleDonorsAsync(id);
            return Ok(new { notified = count });
        }
    }
}
