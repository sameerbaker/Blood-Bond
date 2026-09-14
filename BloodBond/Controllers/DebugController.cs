using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BloodBond.BLL.Service;

namespace BloodBond.Controllers
{
    // Temporary diagnostic controller — answers /api/debug/stripe-config
    // so we can confirm whether the merged IConfiguration (appsettings +
    // user-secrets + env vars) actually contains the Stripe keys.
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IOptions<StripeSettings> _stripe;
        private readonly IConfiguration _config;

        public DebugController(IOptions<StripeSettings> stripe, IConfiguration config)
        {
            _stripe = stripe;
            _config = config;
        }

        [HttpGet("stripe-config")]
        public IActionResult StripeConfig()
        {
            // What the strongly-typed options binder resolved to:
            var opts = _stripe.Value;
            // What the raw IConfiguration says for each key:
            var rawSecret     = _config["Stripe:SecretKey"];
            var rawPublish    = _config["Stripe:PublishableKey"];
            var rawWebhook    = _config["Stripe:WebhookSecret"];
            var rawSuccess    = _config["Stripe:SuccessUrl"];
            var rawCancel     = _config["Stripe:CancelUrl"];
            var environment   = _config["ASPNETCORE_ENVIRONMENT"]
                                 ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                 ?? "(unknown)";

            return Ok(new
            {
                environment,
                options = new
                {
                    opts.SecretKey,
                    opts.PublishableKey,
                    opts.WebhookSecret,
                    opts.SuccessUrl,
                    opts.CancelUrl,
                },
                raw = new
                {
                    SecretKey      = Mask(rawSecret),
                    PublishableKey = Mask(rawPublish),
                    WebhookSecret  = Mask(rawWebhook),
                    SuccessUrl     = rawSuccess,
                    CancelUrl      = rawCancel,
                },
                // Helpful so you can see *where* each value came from.
                providers = new
                {
                    fromUserSecrets = HasValue(rawSecret, "sk_test_")
                                       || HasValue(rawPublish, "pk_test_")
                                       || HasValue(rawWebhook, "whsec_"),
                },
            });
        }

        private static string Mask(string? v)
        {
            if (string.IsNullOrEmpty(v)) return "(empty)";
            if (v.Length <= 12) return v;
            return v.Substring(0, 10) + "…" + v.Substring(v.Length - 4);
        }

        private static bool HasValue(string? v, string prefix) =>
            !string.IsNullOrEmpty(v) && v.StartsWith(prefix);
    }
}
