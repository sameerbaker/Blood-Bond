using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBond.BLL.Service;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace BloodBond.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonetaryDonationsController : ControllerBase
    {
        private readonly IMonetaryDonationService _donationService;
        private readonly StripeSettings _stripe;

        public MonetaryDonationsController(
            IMonetaryDonationService donationService,
            IOptions<StripeSettings> stripeOptions)
        {
            _donationService = donationService;
            _stripe = stripeOptions.Value;
        }

        [HttpPost("create-intent")]
        public async Task<ActionResult<PaymentIntentResponse>> CreateIntent([FromBody] MonetaryDonationRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.CreatePaymentIntentAsync(userId!, request);
            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripe.WebhookSecret
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        await _donationService.ConfirmDonationAsync(paymentIntent.Id, "Succeeded");
                    }
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        await _donationService.ConfirmDonationAsync(paymentIntent.Id, "Failed");
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe webhook error: {ex.Message}");
            }
        }

        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<ActionResult<MonetaryDonationResponse>> Confirm([FromQuery] string paymentIntentId, [FromQuery] string status)
        {
            var result = await _donationService.ConfirmDonationAsync(paymentIntentId, status);
            return Ok(result);
        }

        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<ActionResult> Success([FromQuery] string session_id)
        {
            if (!string.IsNullOrEmpty(session_id))
            {
                await _donationService.ConfirmDonationAsync(session_id, "Succeeded");
            }
            return Ok(new
            {
                message = "Payment succeeded! Thank you for your donation.",
                sessionId = session_id
            });
        }

        [HttpGet("cancel")]
        [AllowAnonymous]
        public IActionResult Cancel()
        {
            return Ok(new { message = "Payment was cancelled." });
        }

        /// <summary>List my own monetary donations.</summary>
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<MonetaryDonationResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _donationService.GetMineAsync(userId!);
            return Ok(list);
        }

        [HttpGet("by-bank/{bankId}")]
        [Authorize(Roles = "BloodBankManager,Admin")]
        public async Task<ActionResult<IEnumerable<MonetaryDonationResponse>>> GetByBank(int bankId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _donationService.GetByBloodBankAsync(bankId, userId!);
            return Ok(list);
        }

        [HttpGet("total/mine")]
        public async Task<ActionResult> GetMyTotal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var total = await _donationService.GetTotalByDonorAsync(userId!);
            return Ok(new { total, currency = "USD" });
        }
    }
}
