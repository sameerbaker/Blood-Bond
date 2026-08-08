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
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService;

        public EligibilityController(IEligibilityService eligibilityService)
        {
            _eligibilityService = eligibilityService;
        }

        [HttpPost]
        public async Task<ActionResult<EligibilityAnswerResponse>> Check([FromBody] EligibilityAnswerRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _eligibilityService.CheckAsync(userId!, request);
            return Ok(result);
        }

        [HttpGet("latest")]
        public async Task<ActionResult<EligibilityAnswerResponse>> GetLatest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _eligibilityService.GetLatestAsync(userId!);
            if (result == null) return NotFound("No eligibility answers yet.");
            return Ok(result);
        }
    }
}
