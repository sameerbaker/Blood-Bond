using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBond.BLL.Service;
using BloodBond.DAL.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodBond.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        /// <summary>List all available badges in the system.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BadgeResponse>>> GetAll()
        {
            var badges = await _badgeService.GetAllBadgesAsync();
            return Ok(badges);
        }

        /// <summary>List the badges the current user has earned.</summary>
        [HttpGet("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserBadgeResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var badges = await _badgeService.GetUserBadgesAsync(userId!);
            return Ok(badges);
        }

        /// <summary>Get the current user's rank/profile on the leaderboard.</summary>
        [HttpGet("me/rank")]
        [Authorize]
        public async Task<ActionResult<LeaderboardEntryResponse>> GetMyRank()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rank = await _badgeService.GetMyRankAsync(userId!);
            if (rank == null) return NotFound();
            return Ok(rank);
        }

        /// <summary>Get the global top donors leaderboard.</summary>
        [HttpGet("leaderboard")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LeaderboardEntryResponse>>> GetLeaderboard([FromQuery] int top = 10)
        {
            var board = await _badgeService.GetLeaderboardAsync(top);
            return Ok(board);
        }
    }
}
