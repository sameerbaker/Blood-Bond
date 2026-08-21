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
    public class RatingsController : ControllerBase
    {
        private readonly IBloodBankRatingService _ratingService;

        public RatingsController(IBloodBankRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>Add or update a rating for a blood bank (1-5 stars).</summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BloodBankRatingResponse>> AddRating([FromBody] AddRatingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _ratingService.AddOrUpdateAsync(userId!, request);
            return Ok(result);
        }

        /// <summary>Get all ratings for a blood bank (anonymous).</summary>
        [HttpGet("by-bank/{bankId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BloodBankRatingResponse>>> GetByBank(int bankId)
        {
            var list = await _ratingService.GetByBloodBankAsync(bankId);
            return Ok(list);
        }

        /// <summary>Get the current user's rating for a specific bank.</summary>
        [HttpGet("mine/{bankId}")]
        [Authorize]
        public async Task<ActionResult<BloodBankRatingResponse?>> GetMine(int bankId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rating = await _ratingService.GetByUserAndBankAsync(userId!, bankId);
            if (rating == null) return NotFound();
            return Ok(rating);
        }

        /// <summary>Get rating stats (average + count) for a blood bank.</summary>
        [HttpGet("stats/{bankId}")]
        [AllowAnonymous]
        public async Task<ActionResult<BloodBankRatingStatsResponse>> GetStats(int bankId)
        {
            var stats = await _ratingService.GetStatsAsync(bankId);
            return Ok(stats);
        }
    }
}
