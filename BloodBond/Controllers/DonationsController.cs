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
    public class DonationsController : ControllerBase
    {
        private readonly IDonationService _donationService;
        private readonly IEligibilityService _eligibilityService;

        public DonationsController(IDonationService donationService, IEligibilityService eligibilityService)
        {
            _donationService = donationService;
            _eligibilityService = eligibilityService;
        }

        // Donor: schedule donation
        [HttpPost]
        public async Task<ActionResult<DonationResponse>> Schedule([FromBody] DonationRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Pre-check eligibility (must have passed)
            var latest = await _eligibilityService.GetLatestAsync(userId!);
            if (latest == null || !latest.Passed)
                return BadRequest("You must pass the eligibility screening first. POST /api/eligibility");

            var result = await _donationService.ScheduleAsync(userId!, request);
            return Ok(result);
        }

        // Donor: my donations
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<DonationResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _donationService.GetByDonorAsync(userId!);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DonationResponse>> GetById(int id)
        {
            var result = await _donationService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<DonationResponse>> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.CancelAsync(id, userId!);
            return Ok(result);
        }

        // BloodBankManager: list bank donations
        [HttpGet("by-bank/{bankId}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<IEnumerable<DonationResponse>>> GetByBank(int bankId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _donationService.GetByBloodBankAsync(bankId, userId!);
            return Ok(list);
        }

        // BloodBankManager: approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<DonationResponse>> Approve(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.ApproveAsync(id, userId!);
            return Ok(result);
        }

        // BloodBankManager: reject
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<DonationResponse>> Reject(int id, [FromBody] CompleteDonationRequest? body)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.RejectAsync(id, userId!, body?.Notes);
            return Ok(result);
        }

        // BloodBankManager: complete → updates inventory + donor points
        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<DonationResponse>> Complete(int id, [FromBody] CompleteDonationRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.CompleteAsync(id, userId!, request);
            return Ok(result);
        }
    }
}
