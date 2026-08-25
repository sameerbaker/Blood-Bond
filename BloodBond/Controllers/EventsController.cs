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
    public class EventsController : ControllerBase
    {
        private readonly IBloodDriveEventService _eventService;

        public EventsController(IBloodDriveEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodDriveEventResponse>>> GetUpcoming()
        {
            var list = await _eventService.GetUpcomingAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BloodDriveEventResponse>> GetById(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodDriveEventResponse>> Create([FromBody] BloodDriveEventRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ev = await _eventService.CreateAsync(userId!, request);
            return Ok(ev);
        }

        /// <summary>Update an event (Manager only).</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<BloodDriveEventResponse>> Update(int id, [FromBody] BloodDriveEventRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ev = await _eventService.UpdateAsync(id, userId!, request);
            return Ok(ev);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _eventService.DeleteAsync(id, userId!);
            return NoContent();
        }

        [HttpGet("by-bank/{bankId}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<IEnumerable<BloodDriveEventResponse>>> GetByBank(int bankId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _eventService.GetByBloodBankAsync(bankId, userId!);
            return Ok(list);
        }

        [HttpPost("{id}/register")]
        [Authorize]
        public async Task<ActionResult<EventAttendanceResponse>> Register(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _eventService.RegisterAsync(id, userId!);
            return Ok(result);
        }

        [HttpPost("{id}/checkin")]
        [Authorize]
        public async Task<ActionResult<EventAttendanceResponse>> CheckIn(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _eventService.CheckInAsync(id, userId!);
            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<EventAttendanceResponse>> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _eventService.CancelAsync(id, userId!);
            return Ok(result);
        }

        [HttpGet("{id}/attendees")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<IEnumerable<EventAttendanceResponse>>> GetAttendees(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _eventService.GetAttendancesAsync(id, userId!);
            return Ok(list);
        }

        [HttpGet("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EventAttendanceResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _eventService.GetMyEventsAsync(userId!);
            return Ok(list);
        }
    }
}
